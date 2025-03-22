namespace TestDataFactoryGenerator.Generation;

internal interface IEitherInformationService
{
    string GetEitherTypeAsString(Type type, Func<Type, string> genericTypeNameGenerator);

    bool IsEither(Type type);
}