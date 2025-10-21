using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PurchaseService.Database;

namespace PurchaseService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseController : ControllerBase
    {
        private readonly PurchaseContext _context;
        private readonly ILogger<PurchaseController> _logger;

        public PurchaseController(PurchaseContext context, ILogger<PurchaseController> logger)
        {
            _context = context;
            _logger = logger;
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