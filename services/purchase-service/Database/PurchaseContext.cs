using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PurchaseService.Domain;

namespace PurchaseService.Database
{
    public class PurchaseContext
    {
        private readonly IMongoDatabase _database;
        
        public PurchaseContext(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            _database = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
        }

        // Expose database for connection testing
        public IMongoDatabase GetDatabase() => _database;

        // Collections
        public IMongoCollection<OrderItem> OrderItems => _database.GetCollection<OrderItem>("OrderItems");
        public IMongoCollection<ShoppingCart> ShoppingCarts => _database.GetCollection<ShoppingCart>("ShoppingCarts");
        
        // TODO: Add more collections here when domain entities are created
        // Example: public IMongoCollection<Purchase> Purchases => _database.GetCollection<Purchase>("Purchases");
    }
}