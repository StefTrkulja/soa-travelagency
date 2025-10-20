using AutoMapper;
using FluentResults;
using TourService.Common;
using TourService.Domain;
using TourService.Domain.RepositoryInterfaces;
using TourService.DTO;

namespace TourService.Services;

public class TourReviewService : ITourReviewService
{
    private readonly ITourReviewRepository _tourReviewRepository;
    private readonly ITourRepository _tourRepository;
    private readonly IMapper _mapper;

    public TourReviewService(ITourReviewRepository tourReviewRepository, ITourRepository tourRepository, IMapper mapper)
    {
        _tourReviewRepository = tourReviewRepository;
        _tourRepository = tourRepository;
        _mapper = mapper;
    }

    public async Task<Result<TourReviewDto>> CreateReviewAsync(CreateTourReviewRequestDto request)
    {
        // Validate that the tour exists
        var tourExists = await _tourRepository.ExistsAsync(request.TourId);
        if (!tourExists)
        {
            return Result.Fail(new Error(FailureCode.NotFound)
                .WithMetadata("reason", FailureCode.NotFound)
                .WithMetadata("message", $"Tour sa ID {request.TourId} nije pronađen"));
        }

        // Check if user already has a review for this tour
        var existingReviewsResult = await _tourReviewRepository.GetByTourAndUserAsync(request.TourId, request.UserId);
        if (existingReviewsResult.IsFailed)
        {
            return Result.Fail(existingReviewsResult.Errors);
        }

        if (existingReviewsResult.Value.Any())
        {
            return Result.Fail(new Error(FailureCode.ValidationError)
                .WithMetadata("reason", FailureCode.ValidationError)
                .WithMetadata("message", "Korisnik je već ostavio recenziju za ovaj tur"));
        }

        var tourReview = _mapper.Map<TourReview>(request);
        var createResult = await _tourReviewRepository.CreateAsync(tourReview);
        
        if (createResult.IsFailed)
        {
            return Result.Fail(createResult.Errors);
        }

        var createdReviewResult = await _tourReviewRepository.GetByIdAsync(createResult.Value.Id);
        if (createdReviewResult.IsFailed)
        {
            return Result.Fail(createdReviewResult.Errors);
        }

        var reviewDto = _mapper.Map<TourReviewDto>(createdReviewResult.Value);
        return Result.Ok(reviewDto);
    }

    public async Task<Result<TourReviewDto>> GetReviewByIdAsync(long id)
    {
        var reviewResult = await _tourReviewRepository.GetByIdAsync(id);
        if (reviewResult.IsFailed)
        {
            return Result.Fail(reviewResult.Errors);
        }

        if (reviewResult.Value == null)
        {
            return Result.Fail(new Error(FailureCode.NotFound)
                .WithMetadata("reason", FailureCode.NotFound)
                .WithMetadata("message", $"Recenzija sa ID {id} nije pronađena"));
        }

        var reviewDto = _mapper.Map<TourReviewDto>(reviewResult.Value);
        return Result.Ok(reviewDto);
    }

    public async Task<Result<List<TourReviewDto>>> GetReviewsByTourIdAsync(long tourId)
    {
        var reviewsResult = await _tourReviewRepository.GetByTourIdAsync(tourId);
        if (reviewsResult.IsFailed)
        {
            return Result.Fail(reviewsResult.Errors);
        }

        var reviewDtos = _mapper.Map<List<TourReviewDto>>(reviewsResult.Value);
        return Result.Ok(reviewDtos);
    }

    public async Task<Result<List<TourReviewDto>>> GetReviewsByUserIdAsync(long userId)
    {
        var reviewsResult = await _tourReviewRepository.GetByUserIdAsync(userId);
        if (reviewsResult.IsFailed)
        {
            return Result.Fail(reviewsResult.Errors);
        }

        var reviewDtos = _mapper.Map<List<TourReviewDto>>(reviewsResult.Value);
        return Result.Ok(reviewDtos);
    }

    public async Task<Result<List<TourReviewDto>>> GetReviewsByTourAndUserAsync(long tourId, long userId)
    {
        var reviewsResult = await _tourReviewRepository.GetByTourAndUserAsync(tourId, userId);
        if (reviewsResult.IsFailed)
        {
            return Result.Fail(reviewsResult.Errors);
        }

        var reviewDtos = _mapper.Map<List<TourReviewDto>>(reviewsResult.Value);
        return Result.Ok(reviewDtos);
    }

    public async Task<Result<List<TourReviewDto>>> GetAllReviewsAsync()
    {
        var reviewsResult = await _tourReviewRepository.GetAllAsync();
        if (reviewsResult.IsFailed)
        {
            return Result.Fail(reviewsResult.Errors);
        }

        var reviewDtos = _mapper.Map<List<TourReviewDto>>(reviewsResult.Value);
        return Result.Ok(reviewDtos);
    }

    public async Task<Result<TourReviewDto>> UpdateReviewAsync(long id, UpdateTourReviewRequestDto request, long userId)
    {
        var reviewResult = await _tourReviewRepository.GetByIdAsync(id);
        if (reviewResult.IsFailed)
        {
            return Result.Fail(reviewResult.Errors);
        }

        if (reviewResult.Value == null)
        {
            return Result.Fail(new Error(FailureCode.NotFound)
                .WithMetadata("reason", FailureCode.NotFound)
                .WithMetadata("message", $"Recenzija sa ID {id} nije pronađena"));
        }

        var review = reviewResult.Value;
        
        // Check if the user is the author of the review
        if (review.UserId != userId)
        {
            return Result.Fail(new Error(FailureCode.Forbidden)
                .WithMetadata("reason", FailureCode.Forbidden)
                .WithMetadata("message", "Nemate dozvolu da menjate ovu recenziju"));
        }

        // Update the review properties
        _mapper.Map(request, review);
        
        var updateResult = await _tourReviewRepository.UpdateAsync(review);
        if (updateResult.IsFailed)
        {
            return Result.Fail(updateResult.Errors);
        }

        var updatedReviewResult = await _tourReviewRepository.GetByIdAsync(id);
        if (updatedReviewResult.IsFailed)
        {
            return Result.Fail(updatedReviewResult.Errors);
        }

        var reviewDto = _mapper.Map<TourReviewDto>(updatedReviewResult.Value);
        return Result.Ok(reviewDto);
    }

    public async Task<Result> DeleteReviewAsync(long id, long userId)
    {
        var reviewResult = await _tourReviewRepository.GetByIdAsync(id);
        if (reviewResult.IsFailed)
        {
            return Result.Fail(reviewResult.Errors);
        }

        if (reviewResult.Value == null)
        {
            return Result.Fail(new Error(FailureCode.NotFound)
                .WithMetadata("reason", FailureCode.NotFound)
                .WithMetadata("message", $"Recenzija sa ID {id} nije pronađena"));
        }

        var review = reviewResult.Value;
        
        // Check if the user is the author of the review
        if (review.UserId != userId)
        {
            return Result.Fail(new Error(FailureCode.Forbidden)
                .WithMetadata("reason", FailureCode.Forbidden)
                .WithMetadata("message", "Nemate dozvolu da obrišete ovu recenziju"));
        }

        return await _tourReviewRepository.DeleteAsync(id);
    }

    public async Task<Result<double?>> GetAverageRatingForTourAsync(long tourId)
    {
        return await _tourReviewRepository.GetAverageRatingForTourAsync(tourId);
    }

    public async Task<Result<int>> GetReviewCountForTourAsync(long tourId)
    {
        return await _tourReviewRepository.GetReviewCountForTourAsync(tourId);
    }
}