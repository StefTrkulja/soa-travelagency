using MongoDB.Driver;
using PurchaseService.Database;
using PurchaseService.Domain;

namespace PurchaseService.Repositories
{
    public interface ITourPurchaseTokenRepository
    {
        Task<TourPurchaseToken> CreateAsync(TourPurchaseToken token);
        Task<TourPurchaseToken?> GetByIdAsync(string id);
        Task<IEnumerable<TourPurchaseToken>> GetAllAsync();
        Task<List<TourPurchaseToken>> GetByUserIdAsync(string userId);
        Task<List<TourPurchaseToken>> GetByTourIdAsync(string tourId);
        Task<TourPurchaseToken?> GetByUserAndTourAsync(string userId, string tourId);
        Task<TourPurchaseToken> UpdateAsync(TourPurchaseToken token);
        Task<bool> DeleteAsync(string id);
    }

    public class TourPurchaseTokenRepository : ITourPurchaseTokenRepository
    {
        private readonly IMongoCollection<TourPurchaseToken> _purchaseTokens;

        public TourPurchaseTokenRepository(PurchaseContext context)
        {
            _purchaseTokens = context.GetDatabase().GetCollection<TourPurchaseToken>("TourPurchaseTokens");
        }

        public async Task<TourPurchaseToken> CreateAsync(TourPurchaseToken token)
        {
            await _purchaseTokens.InsertOneAsync(token);
            return token;
        }

        public async Task<TourPurchaseToken?> GetByIdAsync(string id)
        {
            return await _purchaseTokens.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TourPurchaseToken>> GetAllAsync()
        {
            return await _purchaseTokens.Find(_ => true).ToListAsync();
        }

        public async Task<List<TourPurchaseToken>> GetByUserIdAsync(string userId)
        {
            return await _purchaseTokens.Find(x => x.UserId == userId).ToListAsync();
        }

        public async Task<List<TourPurchaseToken>> GetByTourIdAsync(string tourId)
        {
            return await _purchaseTokens.Find(x => x.TourId == tourId).ToListAsync();
        }

        public async Task<TourPurchaseToken?> GetByUserAndTourAsync(string userId, string tourId)
        {
            return await _purchaseTokens.Find(x => x.UserId == userId && x.TourId == tourId).FirstOrDefaultAsync();
        }

        public async Task<TourPurchaseToken> UpdateAsync(TourPurchaseToken token)
        {
            await _purchaseTokens.ReplaceOneAsync(x => x.Id == token.Id, token);
            return token;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _purchaseTokens.DeleteOneAsync(x => x.Id == id);
            return result.DeletedCount > 0;
        }
    }
}