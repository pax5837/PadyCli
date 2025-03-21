using TestDataForBuilderGeneratorWebApi.Contracts;

namespace TestDataForBuilderGeneratorWebApi.Implementations.Feature1;

internal class Class1 : IInterface1
{
    private readonly IInterface2 _service2;
    private readonly IInterfaceA _serviceA;

    public Class1(
        IInterface2 service2,
        IInterfaceA serviceA)
    {
        _service2 = service2;
        _serviceA = serviceA;
    }

    public Task DoStuffAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}