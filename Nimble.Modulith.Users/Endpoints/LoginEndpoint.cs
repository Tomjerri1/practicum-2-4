using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Identity;

namespace Nimble.Modulith.Users.Endpoints;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginEndpoint : Endpoint<LoginRequest>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public LoginEndpoint(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Post("/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        
        if (user == null || !await _userManager.CheckPasswordAsync(user, req.Password))
        {
            ThrowError("Invalid credentials");
        }

        var jwtSecret = Config["Auth:JwtSecret"]!;
        var token = JwtBearer.CreateToken(
            o =>
            {
                o.SigningKey = jwtSecret;
                o.ExpireAt = DateTime.UtcNow.AddDays(1);
                o.User.Claims.Add(("Email", req.Email));
            });

        await Send.OkAsync(new { Token = token }, ct);
    }
}