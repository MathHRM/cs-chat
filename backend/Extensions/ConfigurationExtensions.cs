using backend.Configs;
using Microsoft.Extensions.DependencyInjection;

namespace backend.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind configuration sections with validation
        services.Configure<JwtSettings>(options =>
        {
            configuration.GetSection("JwtSettings").Bind(options);
            ValidateJwtSettings(options);
        });

        services.Configure<AllowedConfig>(options =>
        {
            configuration.GetSection("AllowedConfig").Bind(options);
            ValidateAllowedConfig(options);
        });

        return services;
    }

    private static void ValidateJwtSettings(JwtSettings jwtSettings)
    {
        if (string.IsNullOrWhiteSpace(jwtSettings.Secret))
            throw new InvalidOperationException("JWT Secret is required and cannot be empty");

        if (string.IsNullOrWhiteSpace(jwtSettings.Issuer))
            throw new InvalidOperationException("JWT Issuer is required and cannot be empty");

        if (string.IsNullOrWhiteSpace(jwtSettings.Audience))
            throw new InvalidOperationException("JWT Audience is required and cannot be empty");

        if (jwtSettings.Secret.Length < 32)
            throw new InvalidOperationException("JWT Secret must be at least 32 characters long for security");
    }

    private static void ValidateAllowedConfig(AllowedConfig allowedConfig)
    {
        if (allowedConfig.Origins == null || allowedConfig.Origins.Length == 0)
            throw new InvalidOperationException("At least one CORS origin must be specified");

        foreach (var origin in allowedConfig.Origins)
        {
            if (string.IsNullOrWhiteSpace(origin))
                throw new InvalidOperationException("CORS origins cannot be null or empty");
        }
    }
}