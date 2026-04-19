using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace Nimble.Modulith.Users.Endpoints;

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterEndpoint : Endpoint<RegisterRequest>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RegisterEndpoint(UserManager<ApplicationUser> userManager)
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
        var user = new ApplicationUser { UserName = req.Email, Email = req.Email };
        var result = await _userManager.CreateAsync(user, req.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                AddError(error.Description);
            
            ThrowIfAnyErrors();
            return;
        }

        await Send.OkAsync(new { Message = "User registered successfully" }, ct);
    }
}