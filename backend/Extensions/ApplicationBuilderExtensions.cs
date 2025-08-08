using backend.Configs;
using backend.src.Hubs;
using Microsoft.AspNetCore.Builder;

namespace backend.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        return app;
    }

    public static IApplicationBuilder UseCustomCors(this IApplicationBuilder app, IConfiguration configuration)
    {
        var allowedConfig = configuration.GetSection("AllowedConfig").Get<AllowedConfig>();

        app.UseCors(cors =>
        {
            cors.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials().WithOrigins(allowedConfig.Origins);
        });
        return app;
    }

    public static IApplicationBuilder UseCustomAuthentication(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }

    public static WebApplication ConfigureEndpoints(this WebApplication app)
    {
        app.MapControllers();
        app.MapHub<HubProvider>("/Hub");
        return app;
    }

    public static WebApplication ConfigureMiddlewarePipeline(this WebApplication app, IConfiguration configuration)
    {
        // Configure the middleware pipeline in the correct order
        app.UseCustomSwagger(app.Environment);
        app.UseHttpsRedirection();
        app.UseRouting();
        app.ConfigureEndpoints();
        app.UseCustomCors(configuration);
        app.UseCustomAuthentication();

        return app;
    }
}