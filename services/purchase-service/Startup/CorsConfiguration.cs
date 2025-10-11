using Microsoft.Net.Http.Headers;

namespace PurchaseService.Startup
{
    public static class CorsConfiguration
    {
        public static IServiceCollection ConfigureCors(this IServiceCollection services, string corsPolicy)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: corsPolicy,
                    builder =>
                    {
                        builder.WithOrigins(ParseCorsOrigins())
                            .WithHeaders(HeaderNames.ContentType, HeaderNames.Authorization, "access_token")
                            .WithMethods("GET", "PUT", "POST", "PATCH", "DELETE", "OPTIONS")
                            .AllowCredentials();
                    });
            });
            return services;
        }

        private static string[] ParseCorsOrigins()
        {
            var corsOrigins = new[] { 
                "http://localhost:4200", 
                "http://localhost:8080", 
                "http://localhost:8081", 
                "http://localhost:5000" 
            };
            var corsOriginsPath = Environment.GetEnvironmentVariable("EXPLORER_CORS_ORIGINS");
            if (File.Exists(corsOriginsPath))
            {
                corsOrigins = File.ReadAllLines(corsOriginsPath);
            }

            return corsOrigins;
        }
    }
}