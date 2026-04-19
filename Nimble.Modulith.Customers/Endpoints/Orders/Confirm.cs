using FastEndpoints;
using Mediator;
using Nimble.Modulith.Customers.Contracts;
using Nimble.Modulith.Customers.Domain.CustomerAggregate;
using Nimble.Modulith.Customers.Domain.Interfaces;
using Nimble.Modulith.Customers.UseCases.Orders.Commands;
using Nimble.Modulith.Email.Contracts;

namespace Nimble.Modulith.Customers.Endpoints.Orders;

public class Confirm(IMediator mediator, IReadRepository<Customer> customerRepo) : EndpointWithoutRequest<OrderDto>
{
    public override void Configure()
    {
        Post("/orders/{id}/confirm");
        AllowAnonymous(); 
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var orderId = Route<int>("id");

        var result = await mediator.Send(new ConfirmOrderCommand(orderId), ct);
        if (!result.IsSuccess)
        {
            AddError("Order not found");
            await Send.ErrorsAsync(statusCode: 404, cancellation: ct);
            return;
        }

        var customer = await customerRepo.GetByIdAsync(result.Value.CustomerId, ct);
        
        var emailBody = $"Dear Customer,\n\nYour order {result.Value.OrderNumber} has been confirmed!\nTotal: ${result.Value.TotalAmount}";
        var emailCommand = new SendEmailCommand(customer!.Email, $"Order Confirmation - {result.Value.OrderNumber}", emailBody);
        
        await mediator.Send(emailCommand, ct);
        
        Response = result.Value;
    }
}