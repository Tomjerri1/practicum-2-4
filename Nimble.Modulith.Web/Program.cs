using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Mediator;
using Nimble.Modulith.Customers;
using Nimble.Modulith.Email;
using Nimble.Modulith.Products;
using Nimble.Modulith.Users;
using Nimble.Modulith.Users.Data;
using Serilog;
using Nimble.Modulith.Reporting;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.AddProductsModuleServices(Log.Logger);
builder.Services.AddCustomersModuleServices(builder.Configuration);
builder.AddReportingModuleServices(builder.Configuration);
builder.Services.AddUsersModule(builder.Configuration);
builder.AddEmailModuleServices(Log.Logger);

builder.Services.AddFastEndpoints()
    .AddAuthenticationJwtBearer(s =>
    {
        s.SigningKey = builder.Configuration["Auth:JwtSecret"]!;
    })
    .AddAuthorization()
    .SwaggerDocument();

builder.Services.AddMediator(options =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
});

builder.AddServiceDefaults();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

await app.EnsureUsersModuleDatabaseAsync();
await app.EnsureProductsModuleDatabaseAsync();
await app.EnsureCustomersModuleDatabaseAsync();
await app.EnsureReportingModuleDatabaseAsync();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints();

app.Run();