namespace TestDataForBuilderGeneratorWebApi.Contracts;

public interface IInterface1
{
    Task DoStuffAsync(Guid id, CancellationToken cancellationToken);
}