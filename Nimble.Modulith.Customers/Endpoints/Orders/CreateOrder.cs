using FastEndpoints;
using Mediator;
using Ardalis.Result;

using Nimble.Modulith.Customers.UseCases.Orders.Commands; 

namespace Nimble.Modulith.Customers.Endpoints.Orders;

public class CreateOrder(IMediator mediator) : Endpoint<CreateOrderCommand, object>
{
    public override void Configure()
    {
        Post("/orders");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateOrderCommand req, CancellationToken ct)
    {
        var result = await mediator.Send(req, ct);

        if (!result.IsSuccess)
        {
            await HttpContext.Response.SendAsync(result.Errors, 400, cancellation: ct);
            return;
        }

        await HttpContext.Response.SendAsync(result.Value, 201, cancellation: ct);
    }
}