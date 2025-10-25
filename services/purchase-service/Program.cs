using PurchaseService.Database;
using PurchaseService.Domain.RepositoryInterfaces;
using PurchaseService.Repositories;
using PurchaseService.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure MongoDB
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<PurchaseContext>();

// Register repositories
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();

// Register services
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.MapControllers();

app.MapGet("/healthz", () => Results.Ok(new { status = "ok", service = "purchase-service" }));

app.Run();

namespace PurchaseService
{
    public partial class Program { }
}