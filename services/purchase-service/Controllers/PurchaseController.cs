using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PurchaseService.Database;
using PurchaseService.Services;
using PurchaseService.DTO;

namespace PurchaseService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseController : ControllerBase
    {
        private readonly PurchaseContext _context;
        private readonly IPurchaseService _purchaseService;
        private readonly ILogger<PurchaseController> _logger;

        public PurchaseController(
            PurchaseContext context, 
            IPurchaseService purchaseService,
            ILogger<PurchaseController> logger)
        {
            _context = context;
            _purchaseService = purchaseService;
            _logger = logger;
        }

        [HttpPost("cart")]
        [Authorize]
        public async Task<ActionResult<PurchaseResponseDto>> PurchaseCart([FromBody] PurchaseCartDto purchaseDto)
        {
            try
            {
                _logger.LogInformation("Purchase cart request received for user: {UserId}", purchaseDto.UserId);
                
                var result = await _purchaseService.PurchaseCartAsync(purchaseDto.UserId);
                
                if (result.Success)
                {
                    _logger.LogInformation("Cart purchased successfully for user: {UserId}", purchaseDto.UserId);
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("Cart purchase failed for user: {UserId}. Reason: {Message}", 
                        purchaseDto.UserId, result.Message);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error purchasing cart for user: {UserId}", purchaseDto.UserId);
                return StatusCode(500, new PurchaseResponseDto 
                { 
                    Success = false, 
                    Message = "An internal error occurred while processing the purchase." 
                });
            }
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<ActionResult<List<TourPurchaseTokenDto>>> GetUserPurchases(string userId)
        {
            try
            {
                _logger.LogInformation("Get user purchases request for user: {UserId}", userId);
                
                var purchases = await _purchaseService.GetUserPurchasesAsync(userId);
                return Ok(purchases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting purchases for user: {UserId}", userId);
                return StatusCode(500, "An internal error occurred while retrieving purchases.");
            }
        }

        [HttpGet("tour/{tourId}")]
        [Authorize]
        public async Task<ActionResult<List<TourPurchaseTokenDto>>> GetTourPurchases(string tourId)
        {
            try
            {
                _logger.LogInformation("Get tour purchases request for tour: {TourId}", tourId);
                
                var purchases = await _purchaseService.GetTourPurchasesAsync(tourId);
                return Ok(purchases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting purchases for tour: {TourId}", tourId);
                return StatusCode(500, "An internal error occurred while retrieving tour purchases.");
            }
        }

        [HttpGet("check/{userId}/{tourId}")]
        [Authorize]
        public async Task<ActionResult<bool>> HasUserPurchasedTour(string userId, string tourId)
        {
            try
            {
                _logger.LogInformation("Check purchase request for user: {UserId}, tour: {TourId}", userId, tourId);
                
                var hasPurchased = await _purchaseService.HasUserPurchasedTourAsync(userId, tourId);
                return Ok(hasPurchased);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking purchase for user: {UserId}, tour: {TourId}", userId, tourId);
                return StatusCode(500, "An internal error occurred while checking purchase status.");
            }
        }

        [HttpGet("test-connection")]
        public async Task<IActionResult> TestDatabaseConnection()
        {
            try
            {
                // Test the database connection
                var database = _context.GetDatabase();
                await database.RunCommandAsync((Command<MongoDB.Bson.BsonDocument>)"{ping:1}");
                
                _logger.LogInformation("Database connection successful");
                return Ok(new { 
                    status = "success", 
                    message = "Connected to MongoDB successfully",
                    databaseName = database.DatabaseNamespace.DatabaseName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database connection failed");
                return StatusCode(500, new { 
                    status = "error", 
                    message = "Failed to connect to MongoDB",
                    error = ex.Message
                });
            }
        }

        [HttpGet("collections")]
        public async Task<IActionResult> ListCollections()
        {
            try
            {
                var database = _context.GetDatabase();
                var collections = await database.ListCollectionNamesAsync();
                var collectionList = await collections.ToListAsync();
                
                return Ok(new { 
                    status = "success", 
                    collections = collectionList,
                    count = collectionList.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list collections");
                return StatusCode(500, new { 
                    status = "error", 
                    message = "Failed to list collections",
                    error = ex.Message
                });
            }
        }
    }
}