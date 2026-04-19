using FastEndpoints;
using Mediator;
using Nimble.Modulith.Customers.Contracts;
using Nimble.Modulith.Customers.Domain.CustomerAggregate;
using Nimble.Modulith.Customers.Domain.Interfaces;
using Nimble.Modulith.Customers.UseCases.Orders.Commands;

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
        
        var orderCreatedEvent = new OrderCreatedEvent(
            result.Value.Id,
            result.Value.CustomerId,
            customer!.Email,
            result.Value.OrderNumber,
            result.Value.OrderDate,
            result.Value.TotalAmount,
            result.Value.Items.Select(i => new OrderItemDetails(
                i.Id,
                i.ProductId,
                i.ProductName,
                i.Quantity,
                i.UnitPrice,
                i.TotalPrice
            )).ToList()
        );

        await mediator.Publish(orderCreatedEvent, ct);
        
        Response = result.Value;
    }
}