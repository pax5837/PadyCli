using System.Collections.Immutable;

namespace TestDataFactoryGenerator.TestData.Served;

internal interface IOrderService
{
    GetOrdersResponse GetOrders(GetOrdersResquest request);

    public record GetOrdersResquest(DateTimeOffset from, DateTimeOffset to, GetOrdersResquest.OrderTypeFilter OrderType)
    {
        public enum OrderTypeFilter
        {
            WithoutDelivery,
            WithDelivery,
        }
    }

    public record GetOrdersResponse(ImmutableArray<Order> Orders);
}