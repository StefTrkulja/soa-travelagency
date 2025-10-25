using PurchaseService.Domain;

namespace PurchaseService.Domain.RepositoryInterfaces
{
    public interface IShoppingCartRepository
    {
        Task<ShoppingCart> CreateAsync(ShoppingCart shoppingCart);
        Task<ShoppingCart?> GetByIdAsync(string id);
        Task<ShoppingCart?> GetActiveByUserIdAsync(string userId);
        Task<IEnumerable<ShoppingCart>> GetByUserIdAsync(string userId);
        Task<IEnumerable<ShoppingCart>> GetAllAsync();
        Task<ShoppingCart> UpdateAsync(ShoppingCart shoppingCart);
        Task<bool> DeleteAsync(string id);
        Task<ShoppingCart?> GetShoppingCartWithOrderItemsAsync(string id);
    }
}