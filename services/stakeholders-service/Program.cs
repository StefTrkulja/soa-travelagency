using Microsoft.EntityFrameworkCore;
using StakeholdersService.Authentication;
using StakeholdersService.Database;
using StakeholdersService.Domain.RepositoryInterfaces;
using StakeholdersService.Repositories;
using StakeholdersService.Services;
using StakeholdersService.Startup;
using Serilog;
using Serilog.Formatting.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ServiceName", "stakeholders-service")
    .WriteTo.Console()
    .WriteTo.File("/logs/stakeholders-service.log", rollingInterval: RollingInterval.Day)
    .WriteTo.File(new ElasticsearchJsonFormatter(), "/logs/stakeholders-service-elastic.json", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

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
builder.Services.AddScoped<IProfileService, ProfileService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
    context.Database.Migrate();
}

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

if (!string.Equals(app.Environment.EnvironmentName, "Docker", StringComparison.OrdinalIgnoreCase))
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseCors(corsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));

try
{
    Log.Information("Starting Stakeholders Service");
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

namespace StakeholdersService
{
    public partial class Program { }
}
