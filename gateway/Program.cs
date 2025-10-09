using Gateway.Configuration;
using Gateway.Models;
using Gateway.Services;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Formatting.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ServiceName", "api-gateway")
    .WriteTo.Console()
    .WriteTo.File("/logs/gateway.log", rollingInterval: RollingInterval.Day)
    .WriteTo.File(new ElasticsearchJsonFormatter(), "/logs/gateway-elastic.json", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Travel Agency Gateway API", 
        Version = "v1",
        Description = "API Gateway za Travel Agency mikroservisnu arhitekturu"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.ConfigureJwtAuthentication();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:8080", "http://localhost:8081", "http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
    });
});

var serviceEndpoints = new ServiceEndpoints
{
    StakeholdersService = Environment.GetEnvironmentVariable("STAKEHOLDERS_SERVICE_URL") ?? "http://localhost:5001",
    TourService = Environment.GetEnvironmentVariable("TOUR_SERVICE_URL") ?? "http://localhost:5002",
    BlogService = Environment.GetEnvironmentVariable("BLOG_SERVICE_URL") ?? "http://localhost:5003",
    FollowerService = Environment.GetEnvironmentVariable("FOLLOWER_SERVICE_URL") ?? "http://localhost:5004"
};

builder.Services.AddSingleton(serviceEndpoints);
builder.Services.AddHttpClient<IServiceProxy, ServiceProxy>();
builder.Services.AddScoped<IServiceProxy, ServiceProxy>();

// gRPC Configuration
var followerGrpcUrl = Environment.GetEnvironmentVariable("FOLLOWER_GRPC_URL") ?? "http://localhost:9091";
builder.Services.AddGrpcClient<Gateway.Grpc.FollowerService.FollowerServiceClient>(options =>
{
    options.Address = new Uri(followerGrpcUrl);
});
builder.Services.AddScoped<IFollowerGrpcClient, FollowerGrpcClient>();

var app = builder.Build();

if (app.Environment.IsDevelopment() ||
    string.Equals(app.Environment.EnvironmentName, "Docker", StringComparison.OrdinalIgnoreCase))
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Travel Agency Gateway API v1");
        c.RoutePrefix = "swagger";
    });
}

if (!string.Equals(app.Environment.EnvironmentName, "Docker", StringComparison.OrdinalIgnoreCase))
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/healthz", () => Results.Ok(new { status = "ok", timestamp = DateTime.UtcNow }));

app.MapGet("/", () => Results.Ok(new 
{ 
    name = "Travel Agency Gateway",
    version = "1.0.0",
    description = "API Gateway za Travel Agency mikroservisnu arhitekturu",
    endpoints = new
    {
        swagger = "/swagger",
        health = "/healthz",
        stakeholders = "/api/gateway/stakeholders/*",
        tours = "/api/gateway/tours/*",
        blogs = "/api/gateway/blogs/*"
    }
}));

try
{
    Log.Information("Starting API Gateway");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

namespace Gateway
{
    public partial class Program { }
}
