using Microsoft.EntityFrameworkCore;
using StakeholdersService.Authentication;
using StakeholdersService.Database;
using StakeholdersService.Domain.RepositoryInterfaces;
using StakeholdersService.Repositories;
using StakeholdersService.Services;
using StakeholdersService.Startup;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger(builder.Configuration);

const string corsPolicy = "_corsPolicy";
builder.Services.ConfigureCors(corsPolicy);
builder.Services.ConfigureAuth();

builder.Services.AddDbContext<StakeholdersContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenGenerator, JwtGenerator>();
builder.Services.AddScoped<IAccountService, AccountService>();

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

namespace StakeholdersService
{
    public partial class Program { }
}
