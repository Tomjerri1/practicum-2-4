using Mediator;
using Nimble.Modulith.Customers.Contracts;
using Nimble.Modulith.Email.Interfaces;

namespace Nimble.Modulith.Email.Integrations;

public class OrderCreatedEventHandler(IQueueService<EmailToSend> queueService) : INotificationHandler<OrderCreatedEvent>
{
    public async ValueTask Handle(OrderCreatedEvent notification, CancellationToken ct)
    {
        var itemsList = string.Join("\n", notification.Items.Select(i => 
            $"- {i.ProductName}: {i.Quantity} x {i.UnitPrice}"));

        var body = $@"
Hello!
Your order {notification.OrderNumber} has been successfully placed.

Order Date: {notification.OrderDate}
Total Amount: {notification.TotalAmount}

Items:
{itemsList}";

        var emailToSend = new EmailToSend(
            notification.CustomerEmail, 
            $"Order Confirmation - {notification.OrderNumber}", 
            body);

        await queueService.EnqueueAsync(emailToSend, ct);
    }
}