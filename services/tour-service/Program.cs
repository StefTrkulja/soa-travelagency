using Microsoft.EntityFrameworkCore;
using TourService.Database;
using TourService.Domain.RepositoryInterfaces;
using TourService.Repositories;
using TourService.Services;
using TourService.Startup;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger(builder.Configuration);

const string corsPolicy = "_corsPolicy";
builder.Services.ConfigureCors(corsPolicy);
builder.Services.ConfigureAuth();

builder.Services.AddDbContext<TourContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Register services
builder.Services.AddScoped<ITourService, TourManagementService>();
builder.Services.AddScoped<ITourRepository, TourRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

var app = builder.Build();

// Swagger i dev tools u Development ili Docker okruženju
if (app.Environment.IsDevelopment() ||
    string.Equals(app.Environment.EnvironmentName, "Docker", StringComparison.OrdinalIgnoreCase))
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

// U kontejneru slušaš HTTP (port 80), zato ne forsiraj HTTPS redirect
if (!string.Equals(app.Environment.EnvironmentName, "Docker", StringComparison.OrdinalIgnoreCase))
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseCors(corsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Jednostavan health endpoint za compose/gateway
app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));

app.Run();

namespace TourService
{
    public partial class Program { }
}
