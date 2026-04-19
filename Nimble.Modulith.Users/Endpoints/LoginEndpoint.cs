using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace Nimble.Modulith.Users.Endpoints;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
}

public class Login : Endpoint<LoginRequest, LoginResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;

    public Login(UserManager<ApplicationUser> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
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
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var roles = await _userManager.GetRolesAsync(user);

        var jwtToken = JwtBearer.CreateToken(
            opts =>
            {
                opts.SigningKey = _config["Auth:JwtSecret"]!;
                opts.ExpireAt = DateTime.UtcNow.AddDays(1);
                
                opts.User.Claims.Add(("Email", user.Email!));
                
                foreach (var role in roles)
                {
                    opts.User.Roles.Add(role);
                }
            });

        Response = new LoginResponse
        {
            Token = jwtToken
        };
    }
}