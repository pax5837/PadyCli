namespace TestDataFactoryGenerator.Generation;

internal interface IUserDefinedGenericsCodeGenerator
{
    bool IsAUserDefinedGenericType(Type type);

    string GenerateInstantiationCode(Type type);

    string MethodName(Type type);
}