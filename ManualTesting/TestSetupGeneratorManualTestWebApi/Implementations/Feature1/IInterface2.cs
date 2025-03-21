namespace TestDataForBuilderGeneratorWebApi.Implementations.Feature1;

internal interface IInterface2
{
    Task<string> FetchDataAsynvc(Guid id, CancellationToken cancellationToken);
}