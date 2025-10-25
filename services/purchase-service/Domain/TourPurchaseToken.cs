using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PurchaseService.Domain
{
    public class TourPurchaseToken
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("tourId")]
        [BsonRepresentation(BsonType.String)]
        public string TourId { get; set; } = string.Empty;

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.String)]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("purchaseDate")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime PurchaseDate { get; set; }

        [BsonElement("tourName")]
        [BsonRepresentation(BsonType.String)]
        public string TourName { get; set; } = string.Empty;

        [BsonElement("tourPrice")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal TourPrice { get; set; }

        public TourPurchaseToken()
        {
            PurchaseDate = DateTime.UtcNow;
        }

        public TourPurchaseToken(string tourId, string userId, string tourName, decimal tourPrice)
        {
            TourId = tourId;
            UserId = userId;
            TourName = tourName;
            TourPrice = tourPrice;
            PurchaseDate = DateTime.UtcNow;
        }
    }
}