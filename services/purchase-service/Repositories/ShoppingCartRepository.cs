using MongoDB.Driver;
using PurchaseService.Database;
using PurchaseService.Domain;
using PurchaseService.Domain.RepositoryInterfaces;

namespace PurchaseService.Repositories
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly IMongoCollection<ShoppingCart> _shoppingCarts;
        private readonly IOrderItemRepository _orderItemRepository;

        public ShoppingCartRepository(PurchaseContext context, IOrderItemRepository orderItemRepository)
        {
            _shoppingCarts = context.GetDatabase().GetCollection<ShoppingCart>("ShoppingCarts");
            _orderItemRepository = orderItemRepository;
        }

        public async Task<ShoppingCart> CreateAsync(ShoppingCart shoppingCart)
        {
            shoppingCart.CreatedAt = DateTime.UtcNow;
            shoppingCart.UpdatedAt = DateTime.UtcNow;
            await _shoppingCarts.InsertOneAsync(shoppingCart);
            return shoppingCart;
        }

        public async Task<ShoppingCart?> GetByIdAsync(string id)
        {
            return await _shoppingCarts.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<ShoppingCart?> GetActiveByUserIdAsync(string userId)
        {
            return await _shoppingCarts
                .Find(x => x.UserId == userId && x.Status == ShoppingCartStatus.Active)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ShoppingCart>> GetByUserIdAsync(string userId)
        {
            return await _shoppingCarts.Find(x => x.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<ShoppingCart>> GetAllAsync()
        {
            return await _shoppingCarts.Find(_ => true).ToListAsync();
        }

        public async Task<ShoppingCart> UpdateAsync(ShoppingCart shoppingCart)
        {
            shoppingCart.UpdatedAt = DateTime.UtcNow;
            await _shoppingCarts.ReplaceOneAsync(x => x.Id == shoppingCart.Id, shoppingCart);
            return shoppingCart;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _shoppingCarts.DeleteOneAsync(x => x.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<ShoppingCart?> GetShoppingCartWithOrderItemsAsync(string id)
        {
            var shoppingCart = await GetByIdAsync(id);
            if (shoppingCart != null)
            {
                var orderItems = await _orderItemRepository.GetByShoppingCartIdAsync(id);
                shoppingCart.OrderItems = orderItems.ToList();
            }
            return shoppingCart;
        }
    }
}