namespace TestDataFactoryGenerator.Generation;

internal interface ICollectionsCodeGenerator
{
    bool IsACollection(Type type);

    string GenerateInstantiationCode(
        Type type,
        HashSet<string> dependencies,
        IParameterInstantiationCodeGenerator parameterInstantiationCodeGenerator);
}