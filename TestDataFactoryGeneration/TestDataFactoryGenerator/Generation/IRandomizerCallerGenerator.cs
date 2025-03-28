namespace TestDataFactoryGenerator.Generation;

internal interface IRandomizerCallerGenerator
{
    bool CanGenerate(Type type);

    string Generate(
        Type type,
        string? parameterName);
}