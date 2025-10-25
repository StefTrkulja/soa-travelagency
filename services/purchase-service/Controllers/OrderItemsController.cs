using Microsoft.AspNetCore.Mvc;
using PurchaseService.DTO;
using PurchaseService.Services;

namespace PurchaseService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderItemsController : ControllerBase
    {
        private readonly IOrderItemService _orderItemService;

        public OrderItemsController(IOrderItemService orderItemService)
        {
            _orderItemService = orderItemService;
        }

        [HttpPost]
        public async Task<ActionResult<OrderItemDto>> CreateOrderItem([FromBody] CreateOrderItemDto dto)
        {
            try
            {
                var orderItem = await _orderItemService.CreateOrderItemAsync(dto);
                return CreatedAtAction(nameof(GetOrderItem), new { id = orderItem.Id }, orderItem);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating order item: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItemDto>> GetOrderItem(string id)
        {
            try
            {
                var orderItem = await _orderItemService.GetOrderItemByIdAsync(id);
                if (orderItem == null)
                    return NotFound($"Order item with ID {id} not found");

                return Ok(orderItem);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving order item: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetAllOrderItems()
        {
            try
            {
                var orderItems = await _orderItemService.GetAllOrderItemsAsync();
                return Ok(orderItems);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving order items: {ex.Message}");
            }
        }

        [HttpGet("shopping-cart/{shoppingCartId}")]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetOrderItemsByShoppingCart(string shoppingCartId)
        {
            try
            {
                var orderItems = await _orderItemService.GetOrderItemsByShoppingCartIdAsync(shoppingCartId);
                return Ok(orderItems);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving order items for shopping cart: {ex.Message}");
            }
        }

        [HttpGet("shopping-cart/{shoppingCartId}/count")]
        public async Task<ActionResult<int>> GetOrderItemsCount(string shoppingCartId)
        {
            try
            {
                var count = await _orderItemService.GetOrderItemsCountByShoppingCartIdAsync(shoppingCartId);
                return Ok(count);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving order items count: {ex.Message}");
            }
        }

        [HttpGet("shopping-cart/{shoppingCartId}/total")]
        public async Task<ActionResult<decimal>> GetTotalPrice(string shoppingCartId)
        {
            try
            {
                var total = await _orderItemService.GetTotalPriceByShoppingCartIdAsync(shoppingCartId);
                return Ok(total);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving total price: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OrderItemDto>> UpdateOrderItem(string id, [FromBody] UpdateOrderItemDto dto)
        {
            try
            {
                var orderItem = await _orderItemService.UpdateOrderItemAsync(id, dto);
                if (orderItem == null)
                    return NotFound($"Order item with ID {id} not found");

                return Ok(orderItem);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating order item: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItem(string id)
        {
            try
            {
                var result = await _orderItemService.DeleteOrderItemAsync(id);
                if (!result)
                    return NotFound($"Order item with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting order item: {ex.Message}");
            }
        }

        [HttpDelete("shopping-cart/{shoppingCartId}")]
        public async Task<IActionResult> DeleteOrderItemsByShoppingCart(string shoppingCartId)
        {
            try
            {
                var result = await _orderItemService.DeleteOrderItemsByShoppingCartIdAsync(shoppingCartId);
                if (!result)
                    return NotFound($"No order items found for shopping cart ID {shoppingCartId}");

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting order items: {ex.Message}");
            }
        }
    }
}