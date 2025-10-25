using PurchaseService.Domain;
using PurchaseService.DTO;
using PurchaseService.Repositories;
using PurchaseService.Domain.RepositoryInterfaces;

namespace PurchaseService.Services
{
    public class PurchaseServiceImpl : IPurchaseService
    {
        private readonly IShoppingCartService _cartService;
        private readonly ITourPurchaseTokenRepository _purchaseTokenRepository;
        private readonly ILogger<PurchaseServiceImpl> _logger;

        public PurchaseServiceImpl(
            IShoppingCartService cartService,
            ITourPurchaseTokenRepository purchaseTokenRepository,
            ILogger<PurchaseServiceImpl> logger)
        {
            _cartService = cartService;
            _purchaseTokenRepository = purchaseTokenRepository;
            _logger = logger;
        }

        public async Task<PurchaseResponseDto> PurchaseCartAsync(string userId)
        {
            try
            {
                _logger.LogInformation("Starting purchase process for user: {UserId}", userId);

                // Get user's cart
                var cartDto = await _cartService.GetActiveShoppingCartByUserIdAsync(userId);
                if (cartDto == null || !cartDto.OrderItems.Any())
                {
                    return new PurchaseResponseDto
                    {
                        Success = false,
                        Message = "Cart is empty or not found."
                    };
                }

                // Create purchase tokens for each cart item
                var purchaseTokens = new List<TourPurchaseToken>();
                var purchaseDate = DateTime.UtcNow;

                foreach (var item in cartDto.OrderItems)
                {
                    // Check if user already purchased this tour
                    var existingPurchase = await _purchaseTokenRepository.GetByUserAndTourAsync(userId, item.TourId);
                    if (existingPurchase != null)
                    {
                        _logger.LogWarning("User {UserId} already purchased tour {TourId}", userId, item.TourId);
                        continue; // Skip already purchased tours
                    }

                    var purchaseToken = new TourPurchaseToken
                    {
                        TourId = item.TourId,
                        UserId = userId,
                        TourName = item.TourName,
                        TourPrice = item.TourPrice,
                        PurchaseDate = purchaseDate
                    };

                    purchaseTokens.Add(purchaseToken);
                }

                if (!purchaseTokens.Any())
                {
                    return new PurchaseResponseDto
                    {
                        Success = false,
                        Message = "All tours in cart were already purchased."
                    };
                }

                // Save purchase tokens to database
                foreach (var token in purchaseTokens)
                {
                    await _purchaseTokenRepository.CreateAsync(token);
                }

                // Clear the cart after successful purchase
                await _cartService.ClearCartAsync(userId);

                _logger.LogInformation("Purchase completed successfully for user: {UserId}, {Count} tours purchased", 
                    userId, purchaseTokens.Count);

                var response = new PurchaseResponseDto
                {
                    Success = true,
                    Message = $"Successfully purchased {purchaseTokens.Count} tours!",
                    TotalAmount = purchaseTokens.Sum(t => t.TourPrice),
                    PurchaseTokens = purchaseTokens.Select(t => new TourPurchaseTokenDto
                    {
                        Id = t.Id,
                        TourId = t.TourId,
                        UserId = t.UserId,
                        TourName = t.TourName,
                        TourPrice = t.TourPrice,
                        PurchaseDate = t.PurchaseDate
                    }).ToList()
                };

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during purchase process for user: {UserId}", userId);
                return new PurchaseResponseDto
                {
                    Success = false,
                    Message = "An error occurred during the purchase process. Please try again."
                };
            }
        }

        public async Task<List<TourPurchaseTokenDto>> GetUserPurchasesAsync(string userId)
        {
            try
            {
                var purchases = await _purchaseTokenRepository.GetByUserIdAsync(userId);
                return purchases.Select(p => new TourPurchaseTokenDto
                {
                    Id = p.Id,
                    TourId = p.TourId,
                    UserId = p.UserId,
                    TourName = p.TourName,
                    TourPrice = p.TourPrice,
                    PurchaseDate = p.PurchaseDate
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting purchases for user: {UserId}", userId);
                return new List<TourPurchaseTokenDto>();
            }
        }

        public async Task<List<TourPurchaseTokenDto>> GetTourPurchasesAsync(string tourId)
        {
            try
            {
                var purchases = await _purchaseTokenRepository.GetByTourIdAsync(tourId);
                return purchases.Select(p => new TourPurchaseTokenDto
                {
                    Id = p.Id,
                    TourId = p.TourId,
                    UserId = p.UserId,
                    TourName = p.TourName,
                    TourPrice = p.TourPrice,
                    PurchaseDate = p.PurchaseDate
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting purchases for tour: {TourId}", tourId);
                return new List<TourPurchaseTokenDto>();
            }
        }

        public async Task<bool> HasUserPurchasedTourAsync(string userId, string tourId)
        {
            try
            {
                var purchase = await _purchaseTokenRepository.GetByUserAndTourAsync(userId, tourId);
                return purchase != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user {UserId} purchased tour {TourId}", userId, tourId);
                return false;
            }
        }
    }
}