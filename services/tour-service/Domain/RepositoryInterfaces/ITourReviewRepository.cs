using FluentResults;
using TourService.Domain;

namespace TourService.Domain.RepositoryInterfaces;

public interface ITourReviewRepository
{
    Task<Result<TourReview>> CreateAsync(TourReview tourReview);
    Task<Result<TourReview?>> GetByIdAsync(long id);
    Task<Result<List<TourReview>>> GetByTourIdAsync(long tourId);
    Task<Result<List<TourReview>>> GetByUserIdAsync(long userId);
    Task<Result<List<TourReview>>> GetByTourAndUserAsync(long tourId, long userId);
    Task<Result<List<TourReview>>> GetAllAsync();
    Task<Result<TourReview>> UpdateAsync(TourReview tourReview);
    Task<Result> DeleteAsync(long id);
    Task<bool> ExistsAsync(long id);
    Task<Result<double?>> GetAverageRatingForTourAsync(long tourId);
    Task<Result<int>> GetReviewCountForTourAsync(long tourId);
}