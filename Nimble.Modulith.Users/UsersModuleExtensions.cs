using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nimble.Modulith.Users.Data;
using Microsoft.AspNetCore.Identity;

namespace Nimble.Modulith.Users;

public static class UsersModuleExtensions
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("UsersDb") 
                               ?? throw new InvalidOperationException("Connection string 'UsersDb' not found.");


        services.AddDbContext<UsersDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions => 
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(3),
                    errorNumbersToAdd: null);
            }));

        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<UsersDbContext>();
        
        services.AddScoped<Nimble.Modulith.Users.Contracts.IUserService, Nimble.Modulith.Users.Services.UserService>();

        return services;
    }
}