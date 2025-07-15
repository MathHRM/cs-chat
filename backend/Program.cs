using System.Text;
using backend.Configs;
using backend.src.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using backend;
using backend.Services;
using backend.Repository;
using backend.Commands;
using backend.Commands.Commands;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Secret)
                )
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/Hub"))
                        context.Token = accessToken;

                    return Task.CompletedTask;
                }
            };
        });
builder.Services.AddAuthorization();

// Add application services
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ChatRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ChatService>();

// Add command services
builder.Services.AddScoped<ICommandResolver, CommandResolver>();
builder.Services.AddScoped<CommandHandler>();
builder.Services.AddScoped<Help>();
builder.Services.AddScoped<Login>();
builder.Services.AddScoped<Register>();
builder.Services.AddScoped<Join>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(cors =>
{
    cors.AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials().WithOrigins("http://localhost:8080");
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<HubProvider>("/Hub");

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