namespace TestDataFactoryGenerator.Generation;

internal interface IParameterInstantiationCodeGenerator
{
    string GenerateParameterInstantiation(
        Type type,
        HashSet<string> dependencies,
        string? parameterName = null);
}