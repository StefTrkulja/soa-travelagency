using PurchaseService.Domain;
using PurchaseService.DTO;

namespace PurchaseService.Mappers
{
    public static class OrderItemMapper
    {
        public static OrderItem ToEntity(CreateOrderItemDto dto)
        {
            return new OrderItem
            {
                TourName = dto.TourName,
                TourPrice = dto.TourPrice,
                TourId = dto.TourId,
                ShoppingCartId = dto.ShoppingCartId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static OrderItemDto ToDto(OrderItem entity)
        {
            return new OrderItemDto
            {
                Id = entity.Id,
                TourName = entity.TourName,
                TourPrice = entity.TourPrice,
                TourId = entity.TourId,
                ShoppingCartId = entity.ShoppingCartId,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        public static void UpdateEntity(OrderItem entity, UpdateOrderItemDto dto)
        {
            if (!string.IsNullOrEmpty(dto.TourName))
                entity.TourName = dto.TourName;

            if (dto.TourPrice.HasValue)
                entity.TourPrice = dto.TourPrice.Value;

            if (!string.IsNullOrEmpty(dto.TourId))
                entity.TourId = dto.TourId;

            if (!string.IsNullOrEmpty(dto.ShoppingCartId))
                entity.ShoppingCartId = dto.ShoppingCartId;

            entity.UpdatedAt = DateTime.UtcNow;
        }
    }
}