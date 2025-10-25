using PurchaseService.Domain.RepositoryInterfaces;
using PurchaseService.DTO;
using PurchaseService.Mappers;
using PurchaseService.Domain;

namespace PurchaseService.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IOrderItemRepository _orderItemRepository;

        public ShoppingCartService(
            IShoppingCartRepository shoppingCartRepository,
            IOrderItemRepository orderItemRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<ShoppingCartDto> CreateShoppingCartAsync(CreateShoppingCartDto dto)
        {
            var shoppingCart = ShoppingCartMapper.ToEntity(dto);
            var createdCart = await _shoppingCartRepository.CreateAsync(shoppingCart);
            return ShoppingCartMapper.ToDto(createdCart);
        }

        public async Task<ShoppingCartDto?> GetShoppingCartByIdAsync(string id)
        {
            var shoppingCart = await _shoppingCartRepository.GetByIdAsync(id);
            if (shoppingCart == null) return null;

            var orderItems = await _orderItemRepository.GetByShoppingCartIdAsync(id);
            return ShoppingCartMapper.ToDto(shoppingCart, orderItems.ToList());
        }

        public async Task<ShoppingCartDto?> GetActiveShoppingCartByUserIdAsync(string userId)
        {
            var shoppingCart = await _shoppingCartRepository.GetActiveByUserIdAsync(userId);
            if (shoppingCart == null) return null;

            var orderItems = await _orderItemRepository.GetByShoppingCartIdAsync(shoppingCart.Id);
            return ShoppingCartMapper.ToDto(shoppingCart, orderItems.ToList());
        }

        public async Task<IEnumerable<ShoppingCartDto>> GetShoppingCartsByUserIdAsync(string userId)
        {
            var shoppingCarts = await _shoppingCartRepository.GetByUserIdAsync(userId);
            var result = new List<ShoppingCartDto>();

            foreach (var cart in shoppingCarts)
            {
                var orderItems = await _orderItemRepository.GetByShoppingCartIdAsync(cart.Id);
                result.Add(ShoppingCartMapper.ToDto(cart, orderItems.ToList()));
            }

            return result;
        }

        public async Task<IEnumerable<ShoppingCartDto>> GetAllShoppingCartsAsync()
        {
            var shoppingCarts = await _shoppingCartRepository.GetAllAsync();
            var result = new List<ShoppingCartDto>();

            foreach (var cart in shoppingCarts)
            {
                var orderItems = await _orderItemRepository.GetByShoppingCartIdAsync(cart.Id);
                result.Add(ShoppingCartMapper.ToDto(cart, orderItems.ToList()));
            }

            return result;
        }

        public async Task<ShoppingCartDto?> UpdateShoppingCartAsync(string id, UpdateShoppingCartDto dto)
        {
            var existingCart = await _shoppingCartRepository.GetByIdAsync(id);
            if (existingCart == null) return null;

            ShoppingCartMapper.UpdateEntity(existingCart, dto);
            var updatedCart = await _shoppingCartRepository.UpdateAsync(existingCart);
            
            var orderItems = await _orderItemRepository.GetByShoppingCartIdAsync(id);
            return ShoppingCartMapper.ToDto(updatedCart, orderItems.ToList());
        }

        public async Task<bool> DeleteShoppingCartAsync(string id)
        {
            // First delete all order items in the cart
            await _orderItemRepository.DeleteByShoppingCartIdAsync(id);
            
            // Then delete the cart itself
            return await _shoppingCartRepository.DeleteAsync(id);
        }

        public async Task<ShoppingCartDto> AddItemToCartAsync(AddItemToCartDto dto, string userId)
        {
            // Check if user has an active shopping cart
            var existingCart = await _shoppingCartRepository.GetActiveByUserIdAsync(userId);
            
            // If no active cart exists, create one
            if (existingCart == null)
            {
                var createCartDto = new CreateShoppingCartDto { UserId = userId };
                existingCart = ShoppingCartMapper.ToEntity(createCartDto);
                existingCart = await _shoppingCartRepository.CreateAsync(existingCart);
            }

            // Create the order item and add it to the cart
            var orderItem = new OrderItem
            {
                TourName = dto.TourName,
                TourPrice = dto.TourPrice,
                TourId = dto.TourId,
                ShoppingCartId = existingCart.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _orderItemRepository.CreateAsync(orderItem);

            // Return the updated cart with items
            var orderItems = await _orderItemRepository.GetByShoppingCartIdAsync(existingCart.Id);
            return ShoppingCartMapper.ToDto(existingCart, orderItems.ToList());
        }

        public async Task<ShoppingCartDto?> GetCartWithItemsAsync(string id)
        {
            return await GetShoppingCartByIdAsync(id);
        }

        public async Task<bool> RemoveItemFromCartAsync(string userId, string orderItemId)
        {
            // Verify the order item belongs to the user's cart
            var userCart = await _shoppingCartRepository.GetActiveByUserIdAsync(userId);
            if (userCart == null) return false;

            var orderItem = await _orderItemRepository.GetByIdAsync(orderItemId);
            if (orderItem == null || orderItem.ShoppingCartId != userCart.Id) return false;

            return await _orderItemRepository.DeleteAsync(orderItemId);
        }

        public async Task<bool> ClearCartAsync(string userId)
        {
            var userCart = await _shoppingCartRepository.GetActiveByUserIdAsync(userId);
            if (userCart == null) return false;

            return await _orderItemRepository.DeleteByShoppingCartIdAsync(userCart.Id);
        }
    }
}