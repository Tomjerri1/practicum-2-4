using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nimble.Modulith.Reporting.Data;

namespace Nimble.Modulith.Reporting;

public static class ReportingModuleExtensions
{
    public static IHostApplicationBuilder AddReportingModuleServices(
        this IHostApplicationBuilder builder,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ReportingDb");
        
        builder.Services.AddDbContext<ReportingDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddScoped<Services.IReportService, Services.ReportService>();

        return builder;
    }

    public static async Task<WebApplication> EnsureReportingModuleDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ReportingDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ReportingDbContext>>();
        var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
        
        try
        {
            if (env.IsDevelopment())
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
            }
            else
            {
                await context.Database.EnsureCreatedAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to ensure reporting database exists");
            throw;
        }
        
        return app;
    }
}