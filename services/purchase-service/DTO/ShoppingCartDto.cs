using System.ComponentModel.DataAnnotations;
using PurchaseService.Domain;

namespace PurchaseService.DTO
{
    public class CreateShoppingCartDto
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string UserId { get; set; } = string.Empty;
    }

    public class UpdateShoppingCartDto
    {
        public ShoppingCartStatus? Status { get; set; }
    }

    public class ShoppingCartDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public ShoppingCartStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
        public decimal TotalPrice { get; set; }
        public int ItemCount { get; set; }
    }

    public class AddItemToCartDto
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
    }
}