using PurchaseService.DTO;

namespace PurchaseService.Services
{
    public interface IPurchaseService
    {
        Task<PurchaseResponseDto> PurchaseCartAsync(string userId);
        Task<List<TourPurchaseTokenDto>> GetUserPurchasesAsync(string userId);
        Task<List<TourPurchaseTokenDto>> GetTourPurchasesAsync(string tourId);
        Task<bool> HasUserPurchasedTourAsync(string userId, string tourId);
    }
}