namespace TestDataFactoryGenerator.Generation;

internal interface ISimpleTypeGenerator
{
    bool CanGenerate(Type type);

    string Generate(
        Type type,
        string? parameterName);
}