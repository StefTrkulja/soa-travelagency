using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PurchaseService.Domain
{
    public class ShoppingCart
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("userId")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("status")]
        public ShoppingCartStatus Status { get; set; } = ShoppingCartStatus.Active;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property - not stored in MongoDB but used for business logic
        [BsonIgnore]
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public enum ShoppingCartStatus
    {
        Active = 0,
        Completed = 1,
        Abandoned = 2
    }
}