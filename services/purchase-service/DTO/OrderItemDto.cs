using System.ComponentModel.DataAnnotations;

namespace PurchaseService.DTO
{
    public class CreateOrderItemDto
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string TourName { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Tour price must be greater than 0")]
        public decimal TourPrice { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string TourId { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string ShoppingCartId { get; set; } = string.Empty;
    }

    public class UpdateOrderItemDto
    {
        [StringLength(100, MinimumLength = 1)]
        public string? TourName { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Tour price must be greater than 0")]
        public decimal? TourPrice { get; set; }

        [StringLength(50, MinimumLength = 1)]
        public string? TourId { get; set; }

        [StringLength(50, MinimumLength = 1)]
        public string? ShoppingCartId { get; set; }
    }

    public class OrderItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string TourName { get; set; } = string.Empty;
        public decimal TourPrice { get; set; }
        public string TourId { get; set; } = string.Empty;
        public string ShoppingCartId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}