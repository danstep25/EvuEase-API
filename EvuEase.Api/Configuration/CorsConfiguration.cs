using Microsoft.AspNetCore.Builder;

namespace EvuEase.Api.Configuration;

public static class CorsConfiguration
{
    public static void AddCorsConfiguration(this WebApplicationBuilder builder)
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        var allowedMethods = builder.Configuration.GetSection("Cors:AllowedMethods").Get<string[]>() ?? new[] { "GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS" };
        var allowedHeaders = builder.Configuration.GetSection("Cors:AllowedHeaders").Get<string[]>() ?? new[] { "*" };
        var allowCredentials = builder.Configuration.GetValue<bool>("Cors:AllowCredentials", true);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins", policy =>
            {
                if (allowedOrigins.Length > 0)
                {
                    policy.WithOrigins(allowedOrigins)
                          .WithMethods(allowedMethods)
                          .WithHeaders(allowedHeaders);
                    
                    if (allowCredentials)
                    {
                        policy.AllowCredentials();
                    }
                }
                else
                {
                    // Fallback: allow any origin (use only in development)
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                }
            });
        });
    }

    public static void UseCorsConfiguration(this WebApplication app)
    {
        app.UseCors("AllowSpecificOrigins");
    }
}

