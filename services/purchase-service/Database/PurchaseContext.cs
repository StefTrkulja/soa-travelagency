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
        public IMongoCollection<TourPurchaseToken> TourPurchaseTokens => _database.GetCollection<TourPurchaseToken>("TourPurchaseTokens");
    }
}