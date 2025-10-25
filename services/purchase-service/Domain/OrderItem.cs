using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PurchaseService.Domain
{
    public class OrderItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("tourName")]
        public string TourName { get; set; } = string.Empty;

        [BsonElement("tourPrice")]
        public decimal TourPrice { get; set; }

        [BsonElement("tourId")]
        public string TourId { get; set; } = string.Empty;

        [BsonElement("shoppingCartId")]
        public string ShoppingCartId { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}