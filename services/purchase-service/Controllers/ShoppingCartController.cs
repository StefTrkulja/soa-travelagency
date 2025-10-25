using Microsoft.AspNetCore.Mvc;
using PurchaseService.DTO;
using PurchaseService.Services;

namespace PurchaseService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        [HttpPost]
        public async Task<ActionResult<ShoppingCartDto>> CreateShoppingCart([FromBody] CreateShoppingCartDto dto)
        {
            try
            {
                var cart = await _shoppingCartService.CreateShoppingCartAsync(dto);
                return CreatedAtAction(nameof(GetShoppingCart), new { id = cart.Id }, cart);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating shopping cart: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShoppingCartDto>> GetShoppingCart(string id)
        {
            try
            {
                var cart = await _shoppingCartService.GetShoppingCartByIdAsync(id);
                if (cart == null)
                    return NotFound($"Shopping cart with ID {id} not found");

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving shopping cart: {ex.Message}");
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ShoppingCartDto>>> GetUserShoppingCarts(string userId)
        {
            try
            {
                var carts = await _shoppingCartService.GetShoppingCartsByUserIdAsync(userId);
                return Ok(carts);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving user shopping carts: {ex.Message}");
            }
        }

        [HttpGet("user/{userId}/active")]
        public async Task<ActionResult<ShoppingCartDto>> GetActiveUserCart(string userId)
        {
            try
            {
                var cart = await _shoppingCartService.GetActiveShoppingCartByUserIdAsync(userId);
                if (cart == null)
                    return NotFound($"No active shopping cart found for user {userId}");

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving active cart: {ex.Message}");
            }
        }

        [HttpPost("add-item")]
        public async Task<ActionResult<ShoppingCartDto>> AddItemToCart([FromBody] AddItemToCartDto dto)
        {
            try
            {
                // Extract user ID from headers (added by gateway)
                var userIdHeader = Request.Headers["X-User-Id"].FirstOrDefault();
                if (string.IsNullOrEmpty(userIdHeader))
                {
                    return BadRequest("User ID not found in request");
                }

                var cart = await _shoppingCartService.AddItemToCartAsync(dto, userIdHeader);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error adding item to cart: {ex.Message}");
            }
        }

        [HttpDelete("user/{userId}/item/{orderItemId}")]
        public async Task<IActionResult> RemoveItemFromCart(string userId, string orderItemId)
        {
            try
            {
                var result = await _shoppingCartService.RemoveItemFromCartAsync(userId, orderItemId);
                if (!result)
                    return NotFound($"Order item not found or doesn't belong to user's cart");

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error removing item from cart: {ex.Message}");
            }
        }

        [HttpDelete("user/{userId}/clear")]
        public async Task<IActionResult> ClearCart(string userId)
        {
            try
            {
                var result = await _shoppingCartService.ClearCartAsync(userId);
                if (!result)
                    return NotFound($"No active cart found for user {userId}");

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error clearing cart: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ShoppingCartDto>> UpdateShoppingCart(string id, [FromBody] UpdateShoppingCartDto dto)
        {
            try
            {
                var cart = await _shoppingCartService.UpdateShoppingCartAsync(id, dto);
                if (cart == null)
                    return NotFound($"Shopping cart with ID {id} not found");

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating shopping cart: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShoppingCart(string id)
        {
            try
            {
                var result = await _shoppingCartService.DeleteShoppingCartAsync(id);
                if (!result)
                    return NotFound($"Shopping cart with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting shopping cart: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShoppingCartDto>>> GetAllShoppingCarts()
        {
            try
            {
                var carts = await _shoppingCartService.GetAllShoppingCartsAsync();
                return Ok(carts);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error retrieving shopping carts: {ex.Message}");
            }
        }
    }
}