using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Nimble.Modulith.Email.Contracts;
using Nimble.Modulith.Users.Infrastructure;
using Nimble.Modulith.Users;

namespace Nimble.Modulith.Users.Endpoints;

public class ResetPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordResponse
{
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
}

public class ResetPassword(UserManager<ApplicationUser> userManager, IMediator mediator) : 
    Endpoint<ResetPasswordRequest, ResetPasswordResponse>
{
    public override void Configure()
    {
        Post("/users/reset-password");
        AllowAnonymous(); 
    }

    public override async Task HandleAsync(ResetPasswordRequest req, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(req.Email);
        
        if (user == null)
        {
            Response = new ResetPasswordResponse { Success = true, Message = "Email sent if user exists." };
            return;
        }

        var newPassword = PasswordGenerator.GeneratePassword();

        await userManager.RemovePasswordAsync(user);
        await userManager.AddPasswordAsync(user, newPassword);
        
        var emailBody = $"Hello,\n\nYour password has been reset successfully.\nYour new temporary password is: {newPassword}\n\nPlease log in and change your password.";
        var emailCommand = new SendEmailCommand(user.Email!, "Password Reset - New Temporary Password", emailBody);

        await mediator.Send(emailCommand, ct);

        Response = new ResetPasswordResponse { Success = true, Message = "Email sent if user exists." };
    }
}