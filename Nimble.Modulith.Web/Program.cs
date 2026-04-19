using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Nimble.Modulith.Users;
using Serilog;

var logger = Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

logger.Information("Starting web host");

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

builder.AddServiceDefaults();

builder.Services.AddFastEndpoints()
    .AddAuthenticationJwtBearer(s =>
    {
        s.SigningKey = builder.Configuration["Auth:JwtSecret"]!;
    })
    .AddAuthorization()
    .SwaggerDocument();

builder.Services.AddUsersModule(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
    dbContext.Database.EnsureCreated();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints()
    .UseSwaggerGen();

app.MapDefaultEndpoints();

app.Run();