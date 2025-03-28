using System.Collections.Immutable;
using System.Reflection;

namespace TestDataFactoryGenerator.Generation.TypesWithConstructors;

internal class TypeWithConstructorsGenerator : ITypeWithConstructorsGenerator
{
    private readonly IParameterInstantiationCodeGenerator _parameterInstantiationCodeGenerator;
    private readonly IProtoCodeGenerator _protoCodeGenerator;
    private readonly ITypeNameGenerator _typeNameGenerator;
    private readonly IUserDefinedGenericsCodeGenerator _userDefinedGenericsCodeGenerator;
    private readonly IProtoInformationService _protoInformationService;
    private readonly TdfGeneratorConfiguration _config;

    private readonly string _leadingUnderscore;

    public TypeWithConstructorsGenerator(
        IParameterInstantiationCodeGenerator parameterInstantiationCodeGenerator,
        IProtoCodeGenerator protoCodeGenerator,
        ITypeNameGenerator typeNameGenerator,
        IUserDefinedGenericsCodeGenerator userDefinedGenericsCodeGenerator,
        IProtoInformationService protoInformationService,
        TdfGeneratorConfiguration config)
    {
        _parameterInstantiationCodeGenerator = parameterInstantiationCodeGenerator;
        _protoCodeGenerator = protoCodeGenerator;
        _typeNameGenerator = typeNameGenerator;
        _userDefinedGenericsCodeGenerator = userDefinedGenericsCodeGenerator;
        _protoInformationService = protoInformationService;
        _config = config;
        _leadingUnderscore = config.LeadingUnderscore();
    }


    public IImmutableList<string> CreateGenerationMethod(
        Type type,
        HashSet<string> dependencies)
    {
        if (type.Namespace is not null)
        {
            dependencies.Add(type.Namespace);
        }

        if (_protoInformationService.IsProto(type))
        {
            return _protoCodeGenerator.GenerateInstantiationCodeForProtobufType(
                type: type,
                dependencies: dependencies,
                parameterInstantiationCodeGenerator: _parameterInstantiationCodeGenerator);
        }

        var constructors = type.GetConstructors()
            .Where(x => x.CustomAttributes.All(y => y.AttributeType.Name != "ObsoleteAttribute"))
            .ToImmutableList();

        return constructors.Count == 1
            ? CreateGenerationCodeForOneConstructor(type, constructors.Single(), dependencies)
            : CreateGenerationCodeForMultipleConstructors(type, constructors, dependencies);
    }

    private IImmutableList<string> CreateGenerationCodeForMultipleConstructors(
        Type t,
        ImmutableList<ConstructorInfo> constructors,
        HashSet<string> dependencies)
    {
        var lines = new Lines(_config);

        lines
            .Add(1, $"public {t.Name} {Definitions.GenerationMethodPrefix}{t.Name}()")
            .Add(1, "{")
            .Add(2, $"var constructorNumber = {_leadingUnderscore}random.Next(0, {constructors.Count()});")
            .Add(2, $"return constructorNumber switch {{");

        for (var j = 0; j < constructors.Count; j++)
        {
            var constructor = constructors[j];

            lines.Add(3, $"{j} => new {t.Name}(");

            var parameters = constructor.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                var endOfLine = (i == parameters.Length - 1) ? ")," : ",";
                var parameterInstantiation = _parameterInstantiationCodeGenerator.GenerateParameterInstantiation(
                    type: parameters[i].ParameterType,
                    dependencies: dependencies);

                lines.Add(4, $"{parameters[i].Name}: {parameterInstantiation}{endOfLine}");
            }
        }

        lines
            .Add(3, $"_ => throw new InvalidOperationException(\"Unexpected constructor number\"),")
            .Add(2, "};")
            .Add(1, "}")
            .AddEmptyLine();

        return lines.ToImmutableList();
    }

    public IImmutableList<string> CreateGenerationCodeForOneConstructor(
        Type type,
        ConstructorInfo constructor,
        HashSet<string> dependencies)
    {
        var lines = new Lines(_config);

        var parameters = constructor.GetParameters();

        var nullabilityKind = type.GetNullabilityKind();

        var parametersWithNoName = parameters.Where(p => p.Name is null).ToImmutableList();
        if (parametersWithNoName.Any())
        {
            throw new InvalidOperationException(
                $"Type {type.Name} has a constructor with parameters that have no names");
        }

        var endOfMethodLine = parameters.Length == 0 ? ")" : string.Empty;

        var returnTypeName = _typeNameGenerator.GetTypeNameForParameter(type);

        var methodName = type.IsGenericType
            ? _userDefinedGenericsCodeGenerator.MethodName(type)
            : $"{Definitions.GenerationMethodPrefix}{type.Name}";

        lines.Add(1, $"public {returnTypeName} {methodName}({endOfMethodLine}");

        for (int i = 0; i < parameters.Length; i++)
        {
            var endOfLine = i == parameters.Length - 1 ? ")" : ",";
            var parameter = parameters[i];
            var typeNameForParameter = _typeNameGenerator.GetTypeNameForParameter(parameter.ParameterType);
            var parameterKind = parameter.GetKind(nullabilityKind);
            if (parameterKind.IsNullable)
            {
                var optionalType = parameterKind.IsValueType
                    ? "OptionalValue"
                    : "OptionalRef";
                lines.Add(2, $"{optionalType}<{typeNameForParameter}> {parameter.Name!.ToCamelCase()} = default{endOfLine}");
            }
            else
            {
                lines.Add(2, $"{typeNameForParameter}? {parameter.Name!.ToCamelCase()} = null{endOfLine}");
            }
        }

        lines.Add(1, "{");

        var endOfConstructorLine = parameters.Length == 0 ? ");" : string.Empty;
        lines.Add(2, $"return new {returnTypeName}({endOfConstructorLine}");


        for (int i = 0; i < parameters.Length; i++)
        {
            var endOfLine = i == parameters.Length - 1 ? ");" : ",";
            var parameter = parameters[i];
            var generateParameterInstantiation = _parameterInstantiationCodeGenerator.GenerateParameterInstantiation(
                type: parameter.ParameterType,
                dependencies: dependencies);
            var methodParameterName = parameter.Name!.ToCamelCase();
            var kind = parameter.GetKind(nullabilityKind);
            if (kind.IsNullable)
            {
                lines.Add(3, $"{parameter.Name}: {methodParameterName}.Unwrap(whenAutoGenerated: () => {generateParameterInstantiation}){endOfLine}");
            }
            else
            {
                lines.Add(3, $"{parameter.Name}: {methodParameterName} ?? {generateParameterInstantiation}{endOfLine}");
            }
        }

        lines
            .Add(1, "}")
            .AddEmptyLine();

        return lines.ToImmutableList();
    }
}