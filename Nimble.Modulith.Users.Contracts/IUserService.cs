namespace Nimble.Modulith.Users.Contracts;

public interface IUserService
{
    Task<UserDetails?> GetUserAsync(string email, CancellationToken ct = default);
}