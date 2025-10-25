using PurchaseService.Domain;
using PurchaseService.DTO;

namespace PurchaseService.Services
{
    public interface IOrderItemService
    {
        Task<OrderItemDto> CreateOrderItemAsync(CreateOrderItemDto dto);
        Task<OrderItemDto?> GetOrderItemByIdAsync(string id);
        Task<IEnumerable<OrderItemDto>> GetOrderItemsByShoppingCartIdAsync(string shoppingCartId);
        Task<IEnumerable<OrderItemDto>> GetAllOrderItemsAsync();
        Task<OrderItemDto?> UpdateOrderItemAsync(string id, UpdateOrderItemDto dto);
        Task<bool> DeleteOrderItemAsync(string id);
        Task<bool> DeleteOrderItemsByShoppingCartIdAsync(string shoppingCartId);
        Task<int> GetOrderItemsCountByShoppingCartIdAsync(string shoppingCartId);
        Task<decimal> GetTotalPriceByShoppingCartIdAsync(string shoppingCartId);
    }
}