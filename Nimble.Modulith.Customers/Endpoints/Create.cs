using FastEndpoints;
using Mediator;
using Ardalis.Result;
using Nimble.Modulith.Customers.UseCases.Customers.Commands;

namespace Nimble.Modulith.Customers.Endpoints;

public class Create(IMediator mediator) : Endpoint<CreateCustomerCommand, object>
{
    public override void Configure()
    {
        Post("/customers");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateCustomerCommand req, CancellationToken ct)
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