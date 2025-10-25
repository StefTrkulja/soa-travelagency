using PurchaseService.Domain;
using PurchaseService.DTO;

namespace PurchaseService.Services
{
    public interface IShoppingCartService
    {
        Task<ShoppingCartDto> CreateShoppingCartAsync(CreateShoppingCartDto dto);
        Task<ShoppingCartDto?> GetShoppingCartByIdAsync(string id);
        Task<ShoppingCartDto?> GetActiveShoppingCartByUserIdAsync(string userId);
        Task<IEnumerable<ShoppingCartDto>> GetShoppingCartsByUserIdAsync(string userId);
        Task<IEnumerable<ShoppingCartDto>> GetAllShoppingCartsAsync();
        Task<ShoppingCartDto?> UpdateShoppingCartAsync(string id, UpdateShoppingCartDto dto);
        Task<bool> DeleteShoppingCartAsync(string id);
        Task<ShoppingCartDto> AddItemToCartAsync(AddItemToCartDto dto, string userId);
        Task<ShoppingCartDto?> GetCartWithItemsAsync(string id);
        Task<bool> RemoveItemFromCartAsync(string userId, string orderItemId);
        Task<bool> ClearCartAsync(string userId);
    }
}