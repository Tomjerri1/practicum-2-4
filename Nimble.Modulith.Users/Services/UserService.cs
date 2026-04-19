using Microsoft.AspNetCore.Identity;
using Nimble.Modulith.Users.Contracts;

namespace Nimble.Modulith.Users.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserDetails?> GetUserAsync(string email, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return null;
        }

        return new UserDetails
        {
            Id = user.Id,
            Email = user.Email!
        };
    }
}