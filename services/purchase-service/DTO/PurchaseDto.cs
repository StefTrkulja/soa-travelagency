using System.ComponentModel.DataAnnotations;

namespace PurchaseService.DTO
{
    public class PurchaseCartDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
    }

    public class PurchaseResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<TourPurchaseTokenDto> PurchaseTokens { get; set; } = new List<TourPurchaseTokenDto>();
        public decimal TotalAmount { get; set; }
    }

    public class TourPurchaseTokenDto
    {
        public string Id { get; set; } = string.Empty;
        public string TourId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public string TourName { get; set; } = string.Empty;
        public decimal TourPrice { get; set; }
    }
}