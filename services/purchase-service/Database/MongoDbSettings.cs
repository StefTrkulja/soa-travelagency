namespace PurchaseService.Database
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string PurchasesCollectionName { get; set; } = null!;
    }
}