using Mediator;
using Nimble.Modulith.Customers.Contracts;

namespace Nimble.Modulith.Email.Integrations;

public class OrderCreatedEventHandler(IEmailSender emailSender) : INotificationHandler<OrderCreatedEvent>
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


        var message = new EmailMessage(
            notification.CustomerEmail, 
            $"Order Confirmation - {notification.OrderNumber}", 
            body);

        await emailSender.SendEmailAsync(message);
    }
}