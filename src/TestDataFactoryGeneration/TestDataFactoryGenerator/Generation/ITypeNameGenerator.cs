namespace TestDataFactoryGenerator.Generation;

internal interface ITypeNameGenerator
{
    string GetTypeNameForParameter(Type parameterType);
}