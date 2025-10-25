using PurchaseService.Domain;
using PurchaseService.DTO;

namespace PurchaseService.Mappers
{
    public static class ShoppingCartMapper
    {
        public static ShoppingCart ToEntity(CreateShoppingCartDto dto)
        {
            return new ShoppingCart
            {
                UserId = dto.UserId,
                Status = ShoppingCartStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static ShoppingCartDto ToDto(ShoppingCart entity, List<OrderItem>? orderItems = null)
        {
            var dto = new ShoppingCartDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };

            if (orderItems != null)
            {
                dto.OrderItems = orderItems.Select(OrderItemMapper.ToDto).ToList();
                dto.TotalPrice = orderItems.Sum(x => x.TourPrice);
                dto.ItemCount = orderItems.Count;
            }

            return dto;
        }

        public static void UpdateEntity(ShoppingCart entity, UpdateShoppingCartDto dto)
        {
            if (dto.Status.HasValue)
                entity.Status = dto.Status.Value;

            entity.UpdatedAt = DateTime.UtcNow;
        }
    }
}