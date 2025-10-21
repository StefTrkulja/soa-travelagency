using FluentResults;
using TourService.DTO;

namespace TourService.Services;

public interface ITourReviewService
{
    Task<Result<TourReviewDto>> CreateReviewAsync(CreateTourReviewRequestDto request);
    Task<Result<TourReviewDto>> GetReviewByIdAsync(long id);
    Task<Result<List<TourReviewDto>>> GetReviewsByTourIdAsync(long tourId);
    Task<Result<List<TourReviewDto>>> GetReviewsByUserIdAsync(long userId);
    Task<Result<List<TourReviewDto>>> GetReviewsByTourAndUserAsync(long tourId, long userId);
    Task<Result<List<TourReviewDto>>> GetAllReviewsAsync();
    Task<Result<TourReviewDto>> UpdateReviewAsync(long id, UpdateTourReviewRequestDto request, long userId);
    Task<Result> DeleteReviewAsync(long id, long userId);
    Task<Result<double?>> GetAverageRatingForTourAsync(long tourId);
    Task<Result<int>> GetReviewCountForTourAsync(long tourId);
}