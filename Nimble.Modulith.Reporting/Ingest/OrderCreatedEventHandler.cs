using Mediator;
using Microsoft.EntityFrameworkCore;
using Nimble.Modulith.Customers.Contracts;
using Nimble.Modulith.Reporting.Data;
using Nimble.Modulith.Reporting.Models;

namespace Nimble.Modulith.Reporting.Ingest;

public class OrderCreatedEventHandler(ReportingDbContext dbContext) : INotificationHandler<OrderCreatedEvent>
{
    public async ValueTask Handle(OrderCreatedEvent notification, CancellationToken ct)
    {
        var dateKey = ConvertToDateKey(notification.OrderDate);

        var customerExists = await dbContext.DimCustomers.AnyAsync(c => c.CustomerId == notification.CustomerId, ct);
        if (!customerExists)
        {
            dbContext.DimCustomers.Add(new DimCustomer
            {
                CustomerId = notification.CustomerId,
                Email = notification.CustomerEmail,
                Name = "Unknown"
            });
        }

        foreach (var item in notification.Items)
        {
            var productExists = await dbContext.DimProducts.AnyAsync(p => p.ProductId == item.ProductId, ct);
            if (!productExists)
            {
                dbContext.DimProducts.Add(new DimProduct
                {
                    ProductId = item.ProductId,
                    Name = item.ProductName
                });
            }

            var factExists = await dbContext.FactOrders.AnyAsync(f => 
                f.OrderId == notification.OrderId && f.OrderItemId == item.Id, ct);
            
            if (!factExists)
            {
                dbContext.FactOrders.Add(new FactOrder
                {
                    DateKey = dateKey,
                    CustomerId = notification.CustomerId,
                    ProductId = item.ProductId,
                    OrderId = notification.OrderId,
                    OrderItemId = item.Id,
                    OrderNumber = notification.OrderNumber,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice,
                    OrderTotalAmount = notification.TotalAmount
                });
            }
        }

        await dbContext.SaveChangesAsync(ct);
    }

    private static int ConvertToDateKey(DateTime date)
    {
        return date.Year * 10000 + date.Month * 100 + date.Day;
    }
}