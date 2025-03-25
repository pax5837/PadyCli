using System.Collections.Immutable;
using System.Reflection;
using TestDataFactoryGenerator.Definitions;

namespace TestDataFactoryGenerator.Generation.TypesWithConstructors;

internal class TypeWithConstructorsGenerator : ITypeWithConstructorsGenerator
{
    private readonly IParameterInstantiationCodeGenerator _parameterInstantiationCodeGenerator;
    private readonly IProtoCodeGenerator _protoCodeGenerator;
    private readonly ITypeNameGenerator _typeNameGenerator;
    private readonly IUserDefinedGenericsCodeGenerator _userDefinedGenericsCodeGenerator;
    private readonly IProtoInformationService _protoInformationService;
    private readonly IRandomizerCallerGenerator _randomizerCallerGenerator;
    private readonly TdfGeneratorConfiguration _config;

    public TypeWithConstructorsGenerator(
        IParameterInstantiationCodeGenerator parameterInstantiationCodeGenerator,
        IProtoCodeGenerator protoCodeGenerator,
        ITypeNameGenerator typeNameGenerator,
        IUserDefinedGenericsCodeGenerator userDefinedGenericsCodeGenerator,
        IProtoInformationService protoInformationService,
        IRandomizerCallerGenerator randomizerCallerGenerator,
        TdfGeneratorConfiguration config)
    {
        _parameterInstantiationCodeGenerator = parameterInstantiationCodeGenerator;
        _protoCodeGenerator = protoCodeGenerator;
        _typeNameGenerator = typeNameGenerator;
        _userDefinedGenericsCodeGenerator = userDefinedGenericsCodeGenerator;
        _protoInformationService = protoInformationService;
        _randomizerCallerGenerator = randomizerCallerGenerator;
        _config = config;
    }


    public IImmutableList<string> CreateGenerationCode(
        Type t,
        HashSet<string> dependencies)
    {
        if (_randomizerCallerGenerator.CanGenerate(t))
        {
            return ImmutableList<string>.Empty;
        }

        if (t.Namespace is not null)
        {
            dependencies.Add(t.Namespace);
        }

        if (_protoInformationService.IsProto(t))
        {
            return _protoCodeGenerator.GenerateInstantiationCodeForProtobufType(t, dependencies, _parameterInstantiationCodeGenerator);
        }

        var constructors = t.GetConstructors()
            .Where(x => x.CustomAttributes.All(y => y.AttributeType.Name != "ObsoleteAttribute"))
            .ToImmutableList();

        return constructors.Count == 1
            ? CreateGenerationCodeForOneConstructor(t, constructors.Single(), dependencies)
            : CreateGenerationCodeForMultipleConstructors(t, constructors, dependencies);
    }

    private IImmutableList<string> CreateGenerationCodeForMultipleConstructors(
        Type t,
        ImmutableList<ConstructorInfo> constructors,
        HashSet<string> dependencies)
    {
        var lines = new List<string>();

        lines.Add($"{_config.Indent}public {t.Name} {Definitions.GenerationMethodPrefix}{t.Name}()");
        lines.Add($"{_config.Indent}{{");
        lines.Add($"{_config.Indent}{_config.Indent}var constructorNumber = _random.Next(0, {constructors.Count()});");
        lines.Add($"{_config.Indent}{_config.Indent}return constructorNumber switch {{");

        for (var j = 0; j < constructors.Count; j++)
        {
            var constructor = constructors[j];

            lines.Add($"{_config.Indent}{_config.Indent}{_config.Indent}{j} => new {t.Name}(");

            var parameters = constructor.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                var endOfLine = i == parameters.Length - 1 ? ")," : ",";
                lines.Add(
                    $"{_config.Indent}{_config.Indent}{_config.Indent}{_config.Indent}{parameters[i].Name}: {_parameterInstantiationCodeGenerator.GenerateParameterInstantiation(parameters[i].ParameterType, dependencies)}{endOfLine}");
            }
        }

        lines.Add($"{_config.Indent}{_config.Indent}{_config.Indent}_ => throw new InvalidOperationException(\"Unexpected constructor number\"),");
        lines.Add($"{_config.Indent}{_config.Indent}}};");
        lines.Add($"{_config.Indent}}}");
        lines.Add(string.Empty);

        return lines.ToImmutableList();
    }

    public IImmutableList<string> CreateGenerationCodeForOneConstructor(
        Type type,
        ConstructorInfo constructor,
        HashSet<string> dependencies)
    {
        var lines = new List<string>();

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

        lines.Add($"{_config.Indent}public {returnTypeName} {methodName}({endOfMethodLine}");

        for (int i = 0; i < parameters.Length; i++)
        {
            var endOfLine = i == parameters.Length - 1 ? ")" : ",";
            var parameter = parameters[i];
            var typeNameForParameter = _typeNameGenerator.GetTypeNameForParameter(parameter.ParameterType);
            var parameterKind = parameter.GetKind(nullabilityKind);
            if (parameterKind.IsNullable)
            {
                dependencies.Add(typeof(InternalOption).Namespace);
                var optionalType = parameterKind.IsValueType
                    ? "OptionalValue"
                    : "OptionalRef";
                lines.Add(
                    $"{_config.Indent}{_config.Indent}{optionalType}<{typeNameForParameter}> {parameter.Name!.ToCamelCase()} = default{endOfLine}");
            }
            else
            {
                lines.Add(
                    $"{_config.Indent}{_config.Indent}{typeNameForParameter}? {parameter.Name!.ToCamelCase()} = null{endOfLine}");
            }
        }

        lines.Add($"{_config.Indent}{{");

        var endOfConstructorLine = parameters.Length == 0 ? ");" : string.Empty;
        lines.Add($"{_config.Indent}{_config.Indent}return new {returnTypeName}({endOfConstructorLine}");


        for (int i = 0; i < parameters.Length; i++)
        {
            var endOfLine = i == parameters.Length - 1 ? ");" : ",";
            var parameter = parameters[i];
            var generateParameterInstantiation = _parameterInstantiationCodeGenerator.GenerateParameterInstantiation(
                parameter.ParameterType,
                dependencies);
            var methodParameterName = parameter.Name!.ToCamelCase();
            var kind = parameter.GetKind(nullabilityKind);
            if (kind.IsNullable)
            {
                lines.Add(
                    $"{_config.Indent}{_config.Indent}{_config.Indent}{parameter.Name}: {methodParameterName}.Unwrap(whenAutoGenerated: () => {generateParameterInstantiation}){endOfLine}");
            }
            else
            {
                lines.Add(
                    $"{_config.Indent}{_config.Indent}{_config.Indent}{parameter.Name}: {methodParameterName} ?? {generateParameterInstantiation}{endOfLine}");
            }
        }

        lines.Add($"{_config.Indent}}}");
        lines.Add(string.Empty);

        return lines.ToImmutableList();
    }
}