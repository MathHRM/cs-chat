using backend.Configs;
using backend.Extensions;

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