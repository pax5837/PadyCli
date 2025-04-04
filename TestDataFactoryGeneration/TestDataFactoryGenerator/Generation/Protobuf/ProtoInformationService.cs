namespace TestDataFactoryGenerator.Generation.Protobuf;

internal class ProtoInformationService : IProtoInformationService
{
    private readonly TdfGeneratorConfiguration _tdfGeneratorConfiguration;

    public ProtoInformationService(TdfGeneratorConfiguration tdfGeneratorConfiguration)
    {
        _tdfGeneratorConfiguration = tdfGeneratorConfiguration;
    }

    public bool IsWellKnownProtobufType(Type type)
    {
        return type.FullName is not null &&
               _tdfGeneratorConfiguration.CustomInstantiationForWellKnownProtobufTypesByFullName.ContainsKey(type.FullName);
    }

    public bool IsProto(Type type)
    {
        return type.FullName is not null
               && type.GetInterfaces().Any(intf =>
                   intf.FullName is not null && intf.FullName.StartsWith("Google.Protobuf.IMessage") &&
                   intf.FullName.Contains(type.FullName));
    }

    public bool IsProtoRepeatedField(Type type)
    {
        return type.FullName is not null
               && type.FullName.StartsWith("Google.Protobuf.Collections.RepeatedField");
    }

    public string GetParameterTypeNameForRepeatedField(string typeName)
    {
        return $"IEnumerable<{typeName}>";
    }
}