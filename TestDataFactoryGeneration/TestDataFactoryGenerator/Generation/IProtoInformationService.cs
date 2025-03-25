namespace TestDataFactoryGenerator.Generation;

internal interface IProtoInformationService
{
    bool IsWellKnownProtobufType(Type type);

    bool IsProto(Type type);

    bool IsProtoRepeatedField(Type type);

    string GetParameterTypeNameForRepeatedField(string typeName);
}