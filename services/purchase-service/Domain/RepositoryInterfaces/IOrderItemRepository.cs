using PurchaseService.Domain;

namespace PurchaseService.Domain.RepositoryInterfaces
{
    public interface IOrderItemRepository
    {
        Task<OrderItem> CreateAsync(OrderItem orderItem);
        Task<OrderItem?> GetByIdAsync(string id);
        Task<IEnumerable<OrderItem>> GetByShoppingCartIdAsync(string shoppingCartId);
        Task<IEnumerable<OrderItem>> GetAllAsync();
        Task<OrderItem> UpdateAsync(OrderItem orderItem);
        Task<bool> DeleteAsync(string id);
        Task<bool> DeleteByShoppingCartIdAsync(string shoppingCartId);
        Task<int> GetCountByShoppingCartIdAsync(string shoppingCartId);
        Task<decimal> GetTotalPriceByShoppingCartIdAsync(string shoppingCartId);
    }
}