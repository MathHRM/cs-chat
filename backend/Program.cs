using backend;
using backend.Configs;
using backend.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container using extension methods
builder.Services
    .AddConfiguration(builder.Configuration)
    .AddDatabase(builder.Configuration)
    .AddInfrastructure()
    .AddCustomAuthentication(builder.Configuration)
    .AddCustomCors(builder.Configuration)
    .AddRepositories()
    .AddApplicationServices()
    .AddCommands();

var app = builder.Build();

// Configure the HTTP request pipeline using extension methods
app.ConfigureMiddlewarePipeline();

// Apply pending EF Core migrations on startup.
// Disable with env var: DisableMigrationsOnStartup=true
if (!builder.Configuration.GetValue<bool>("DisableMigrationsOnStartup"))
{
    var migrationConnectionString =
        builder.Configuration.GetConnectionString("MigrationsConnection")
        ?? builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("No connection string configured for migrations.");

    var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
        .UseNpgsql(migrationConnectionString, o => o.CommandTimeout(300));

    using var db = new AppDbContext(optionsBuilder.Options);

    try
    {
        db.Database.Migrate();
        Logger.Info("Database migrations applied successfully.");
    }
    catch (Exception ex)
    {
        Logger.Error($"Database migration failed: {ex.Message}");
        throw;
    }
}

app.Run();

public static class Logger
{
    public static void Info(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n\nInfo: {message}\n\n");
        Console.ResetColor();
    }

    public static void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n\nError: {message}\n\n");
        Console.ResetColor();
    }
}
