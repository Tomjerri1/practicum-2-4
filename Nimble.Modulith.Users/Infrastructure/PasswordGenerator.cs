namespace Nimble.Modulith.Users.Infrastructure;

public static class PasswordGenerator
{
    public static string GeneratePassword()
    {

        var baseString = Guid.NewGuid().ToString("N")[..8];
        

        return baseString + "A1!"; 
    }
}