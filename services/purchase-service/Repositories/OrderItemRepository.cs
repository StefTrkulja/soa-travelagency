using MongoDB.Driver;
using PurchaseService.Database;
using PurchaseService.Domain;
using PurchaseService.Domain.RepositoryInterfaces;

namespace PurchaseService.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly IMongoCollection<OrderItem> _orderItems;

        public OrderItemRepository(PurchaseContext context)
        {
            _orderItems = context.GetDatabase().GetCollection<OrderItem>("OrderItems");
        }

        public async Task<OrderItem> CreateAsync(OrderItem orderItem)
        {
            orderItem.CreatedAt = DateTime.UtcNow;
            orderItem.UpdatedAt = DateTime.UtcNow;
            await _orderItems.InsertOneAsync(orderItem);
            return orderItem;
        }

        public async Task<OrderItem?> GetByIdAsync(string id)
        {
            return await _orderItems.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<OrderItem>> GetByShoppingCartIdAsync(string shoppingCartId)
        {
            return await _orderItems.Find(x => x.ShoppingCartId == shoppingCartId).ToListAsync();
        }

        public async Task<IEnumerable<OrderItem>> GetAllAsync()
        {
            return await _orderItems.Find(_ => true).ToListAsync();
        }

        public async Task<OrderItem> UpdateAsync(OrderItem orderItem)
        {
            orderItem.UpdatedAt = DateTime.UtcNow;
            await _orderItems.ReplaceOneAsync(x => x.Id == orderItem.Id, orderItem);
            return orderItem;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _orderItems.DeleteOneAsync(x => x.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<bool> DeleteByShoppingCartIdAsync(string shoppingCartId)
        {
            var result = await _orderItems.DeleteManyAsync(x => x.ShoppingCartId == shoppingCartId);
            return result.DeletedCount > 0;
        }

        public async Task<int> GetCountByShoppingCartIdAsync(string shoppingCartId)
        {
            return (int)await _orderItems.CountDocumentsAsync(x => x.ShoppingCartId == shoppingCartId);
        }

        public async Task<decimal> GetTotalPriceByShoppingCartIdAsync(string shoppingCartId)
        {
            var orderItems = await _orderItems.Find(x => x.ShoppingCartId == shoppingCartId).ToListAsync();
            return orderItems.Sum(x => x.TourPrice);
        }
    }
}