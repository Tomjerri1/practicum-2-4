using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace Nimble.Modulith.Users.Endpoints;

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterResponse
{
    public string Message { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
}

public class Register : Endpoint<RegisterRequest, RegisterResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public Register(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Post("/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var user = new ApplicationUser
        {
            UserName = req.Email,
            Email = req.Email
        };

        var result = await _userManager.CreateAsync(user, req.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                AddError(error.Description);
            }
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        Response = new RegisterResponse
        {
            Message = "User registered successfully",
            Id = user.Id
        };
    }
}