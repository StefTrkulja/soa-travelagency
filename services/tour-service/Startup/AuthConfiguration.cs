using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TourService.Startup;

public static class AuthConfiguration
{
    public static IServiceCollection ConfigureAuth(this IServiceCollection services)
    {
        ConfigureAuthentication(services);
        ConfigureAuthorizationPolicies(services);
        return services;
    }

    private static void ConfigureAuthentication(IServiceCollection services)
    {
        var key = Environment.GetEnvironmentVariable("JWT_KEY") ?? "explorer_secret_key_very_long_and_secure_key_for_production";
        var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "travel-agency";
        var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "travel-agency-users";

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ClockSkew = TimeSpan.FromMinutes(5)
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Append("AuthenticationTokens-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
    }

    private static void ConfigureAuthorizationPolicies(IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("administratorPolicy", policy => policy.RequireRole("Administrator"));
            options.AddPolicy("authorPolicy", policy => policy.RequireRole("Author"));
            options.AddPolicy("touristPolicy", policy => policy.RequireRole("Tourist"));
        });
    }
}