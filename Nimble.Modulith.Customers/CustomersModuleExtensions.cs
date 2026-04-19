using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nimble.Modulith.Customers.Data;
using Nimble.Modulith.Customers.Domain.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Nimble.Modulith.Customers;

public static class CustomersModuleExtensions
{
    public static IServiceCollection AddCustomersModuleServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CustomersDb");
        services.AddDbContext<CustomersDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));

        return services;
    }
    public static async Task<IHost> EnsureCustomersModuleDatabaseAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CustomersDbContext>();
        

        await context.Database.EnsureCreatedAsync(); 
        
        return host;
    }
}