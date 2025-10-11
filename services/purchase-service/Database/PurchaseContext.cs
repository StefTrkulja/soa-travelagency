using Microsoft.Extensions.Options;
using MongoDB.Driver;

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

        // TODO: Add collections here when domain entities are created
        // Example: public IMongoCollection<Purchase> Purchases => _database.GetCollection<Purchase>("Purchases");
    }
}