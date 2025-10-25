using PurchaseService.Domain.RepositoryInterfaces;
using PurchaseService.DTO;
using PurchaseService.Mappers;

namespace PurchaseService.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderItemService(IOrderItemRepository orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;
        }

        public async Task<OrderItemDto> CreateOrderItemAsync(CreateOrderItemDto dto)
        {
            var orderItem = OrderItemMapper.ToEntity(dto);
            var createdOrderItem = await _orderItemRepository.CreateAsync(orderItem);
            return OrderItemMapper.ToDto(createdOrderItem);
        }

        public async Task<OrderItemDto?> GetOrderItemByIdAsync(string id)
        {
            var orderItem = await _orderItemRepository.GetByIdAsync(id);
            return orderItem != null ? OrderItemMapper.ToDto(orderItem) : null;
        }

        public async Task<IEnumerable<OrderItemDto>> GetOrderItemsByShoppingCartIdAsync(string shoppingCartId)
        {
            var orderItems = await _orderItemRepository.GetByShoppingCartIdAsync(shoppingCartId);
            return orderItems.Select(OrderItemMapper.ToDto);
        }

        public async Task<IEnumerable<OrderItemDto>> GetAllOrderItemsAsync()
        {
            var orderItems = await _orderItemRepository.GetAllAsync();
            return orderItems.Select(OrderItemMapper.ToDto);
        }

        public async Task<OrderItemDto?> UpdateOrderItemAsync(string id, UpdateOrderItemDto dto)
        {
            var existingOrderItem = await _orderItemRepository.GetByIdAsync(id);
            if (existingOrderItem == null)
                return null;

            OrderItemMapper.UpdateEntity(existingOrderItem, dto);
            var updatedOrderItem = await _orderItemRepository.UpdateAsync(existingOrderItem);
            return OrderItemMapper.ToDto(updatedOrderItem);
        }

        public async Task<bool> DeleteOrderItemAsync(string id)
        {
            return await _orderItemRepository.DeleteAsync(id);
        }

        public async Task<bool> DeleteOrderItemsByShoppingCartIdAsync(string shoppingCartId)
        {
            return await _orderItemRepository.DeleteByShoppingCartIdAsync(shoppingCartId);
        }

        public async Task<int> GetOrderItemsCountByShoppingCartIdAsync(string shoppingCartId)
        {
            return await _orderItemRepository.GetCountByShoppingCartIdAsync(shoppingCartId);
        }

        public async Task<decimal> GetTotalPriceByShoppingCartIdAsync(string shoppingCartId)
        {
            return await _orderItemRepository.GetTotalPriceByShoppingCartIdAsync(shoppingCartId);
        }
    }
}