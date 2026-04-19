using FastEndpoints;

namespace Nimble.Modulith.Users.Endpoints;

public class ProfileEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/profile");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var email = User.FindFirst("Email")?.Value;
        await Send.OkAsync(new { Email = email }, ct);
    }
}