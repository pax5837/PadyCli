using System.Collections.Immutable;
using System.Reflection;
namespace TestDataFactoryGenerator.Generation.Protobuf;

internal class ProtoCodeGenerator : IProtoCodeGenerator
{
    private readonly ITypeNameGenerator _typeNameGenerator;
    private readonly IProtoInformationService _protoInformationService;
    private readonly TdfGeneratorConfiguration _config;

    private readonly string _leadingUnderscore;

    public ProtoCodeGenerator(
        ITypeNameGenerator typeNameGenerator,
        IProtoInformationService protoInformationService,
        TdfGeneratorConfiguration config)
    {
        _typeNameGenerator = typeNameGenerator;
        _protoInformationService = protoInformationService;
        _config = config;
        _leadingUnderscore = config.LeadingUnderscore();
    }

    public IImmutableList<Type> GetNestedTypes(
        Type type)
    {
        return GetNestedProperties(type).Select(x => x.PropertyType).ToImmutableList();
    }

    private ImmutableList<PropertyInfo> GetNestedProperties(Type type)
    {
        return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetField)
            .Where(p => p.CanWrite || _protoInformationService.IsProtoRepeatedField(p.PropertyType))
            .ToImmutableList();
    }

    public string GenerateInstantiationForWellKnownProtobufType(
        Type type,
        HashSet<string> dependencies)
    {
        dependencies.Add(_config.CustomInstantiationForWellKnownProtobufTypesByFullName[type.FullName!].NamespaceToAdd);

        return _config.CustomInstantiationForWellKnownProtobufTypesByFullName[type.FullName!].InstantiationCode;
    }

    public IImmutableList<string> GenerateInstantiationCodeForProtobufType(
        Type type,
        HashSet<string> dependencies,
        IParameterInstantiationCodeGenerator parameterInstantiationCodeGenerator)
    {
        if (type.Namespace is not null && !_protoInformationService.IsProtoRepeatedField(type))
        {
            dependencies.Add(type.Namespace);
        }

        var lines = new List<string>();

        var nestedProperties = GetNestedProperties(type);

        var endOfMethodLine = nestedProperties.Count == 0 ? "()" : "(";
        lines.Add($"{_config.Indent}public {type.Name} {Definitions.GenerationMethodPrefix}{type.Name}{endOfMethodLine}");

        for (int i = 0; i < nestedProperties.Count; i++)
        {
            var endOfLine = i == nestedProperties.Count - 1 ? ")" : ",";
            lines.Add(
                $"{_config.Indents(2)}{_typeNameGenerator.GetTypeNameForParameter(nestedProperties[i].PropertyType)}? {nestedProperties[i].Name.ToCamelCase()} = null{endOfLine}");
        }

        lines.Add($"{_config.Indent}{{");

        var nestedSingleProperties = GetNestedProperties(type).Where(p => !_protoInformationService.IsProtoRepeatedField(p.PropertyType))
            .ToImmutableList();
        var endOfConstructorLine = nestedSingleProperties.Count == 0 ? "();" : string.Empty;
        lines.Add($"{_config.Indents(2)}var generated = new {type.Name}{endOfConstructorLine}");

        if (nestedSingleProperties.Count > 0)
        {
            lines.Add($"{_config.Indent}{_config.Indent}{{");
            for (int i = 0; i < nestedSingleProperties.Count; i++)
            {
                var propertyName = nestedSingleProperties[i].Name;
                var propertyType = nestedSingleProperties[i].PropertyType;
                var methodParameterName = propertyName.ToCamelCase();

                lines.Add(
                    $"{_config.Indents(3)}{propertyName} = {methodParameterName} ?? {parameterInstantiationCodeGenerator.GenerateParameterInstantiation(propertyType, dependencies)},");
            }

            lines.Add($"{_config.Indents(2)}}};");
        }

        var nestedRepeatedProperties =
            GetNestedProperties(type).Where(p => _protoInformationService.IsProtoRepeatedField(p.PropertyType)).ToImmutableList();
        if (nestedRepeatedProperties.Any())
        {
            lines.Add(string.Empty);
            for (int i = 0; i < nestedRepeatedProperties.Count; i++)
            {
                var propertyName = nestedRepeatedProperties[i].Name;
                var propertyType = nestedRepeatedProperties[i].PropertyType;

                lines.Add(
                    $"{_config.Indents(2)}generated.{propertyName}.AddRange({propertyName.ToCamelCase()} ?? {parameterInstantiationCodeGenerator.GenerateParameterInstantiation(propertyType, dependencies)}));");
            }
        }

        lines.Add(string.Empty);
        lines.Add($"{_config.Indents(2)}return generated;");
        lines.Add($"{_config.Indent}}}");
        lines.Add(string.Empty);

        return lines.ToImmutableList();
    }

    public string GenerateInstantiationCodeForProtobufRepeatedType(
        Type type,
        HashSet<string> dependencies,
        IParameterInstantiationCodeGenerator parameterInstantiationCodeGenerator)
    {
        var genericType = type.GenericTypeArguments.Single();
        dependencies.Add("System.Linq");
        dependencies.Add("System.Collections.Generic");
        return
            $"Enumerable.Range(1, {_leadingUnderscore}random.Next(0, GetZeroBiasedCount())).Select(_ => {parameterInstantiationCodeGenerator.GenerateParameterInstantiation(genericType, dependencies)}";
    }
}