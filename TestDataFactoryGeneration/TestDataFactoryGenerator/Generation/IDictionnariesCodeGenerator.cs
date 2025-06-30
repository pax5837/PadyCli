namespace TestDataFactoryGenerator.Generation;

internal interface IDictionnariesCodeGenerator
{
    bool IsADictionary(Type type);

    string GenerateInstantiationCode(
        Type type,
        HashSet<string> dependencies,
        IParameterInstantiationCodeGenerator parameterInstantiationCodeGenerator);
}