using FluentResults;
using Microsoft.EntityFrameworkCore;
using TourService.Common;
using TourService.Database;
using TourService.Domain;
using TourService.Domain.RepositoryInterfaces;

namespace TourService.Repositories;

public class TourReviewRepository : ITourReviewRepository
{
    private readonly TourContext _context;

    public TourReviewRepository(TourContext context)
    {
        _context = context;
    }

    public async Task<Result<TourReview>> CreateAsync(TourReview tourReview)
    {
        try
        {
            _context.TourReviews.Add(tourReview);
            await _context.SaveChangesAsync();
            return Result.Ok(tourReview);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result<TourReview?>> GetByIdAsync(long id)
    {
        try
        {
            var tourReview = await _context.TourReviews
                .Include(tr => tr.Tour)
                .FirstOrDefaultAsync(tr => tr.Id == id);
            
            return Result.Ok(tourReview);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result<List<TourReview>>> GetByTourIdAsync(long tourId)
    {
        try
        {
            var tourReviews = await _context.TourReviews
                .Include(tr => tr.Tour)
                .Where(tr => tr.TourId == tourId)
                .OrderByDescending(tr => tr.CreatedAt)
                .ToListAsync();
            
            return Result.Ok(tourReviews);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result<List<TourReview>>> GetByUserIdAsync(long userId)
    {
        try
        {
            var tourReviews = await _context.TourReviews
                .Include(tr => tr.Tour)
                .Where(tr => tr.UserId == userId)
                .OrderByDescending(tr => tr.CreatedAt)
                .ToListAsync();
            
            return Result.Ok(tourReviews);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result<List<TourReview>>> GetByTourAndUserAsync(long tourId, long userId)
    {
        try
        {
            var tourReviews = await _context.TourReviews
                .Include(tr => tr.Tour)
                .Where(tr => tr.TourId == tourId && tr.UserId == userId)
                .OrderByDescending(tr => tr.CreatedAt)
                .ToListAsync();
            
            return Result.Ok(tourReviews);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result<List<TourReview>>> GetAllAsync()
    {
        try
        {
            var tourReviews = await _context.TourReviews
                .Include(tr => tr.Tour)
                .OrderByDescending(tr => tr.CreatedAt)
                .ToListAsync();
            
            return Result.Ok(tourReviews);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result<TourReview>> UpdateAsync(TourReview tourReview)
    {
        try
        {
            tourReview.UpdatedAt = DateTime.UtcNow;
            _context.TourReviews.Update(tourReview);
            await _context.SaveChangesAsync();
            return Result.Ok(tourReview);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result> DeleteAsync(long id)
    {
        try
        {
            var tourReview = await _context.TourReviews.FindAsync(id);
            if (tourReview == null)
            {
                return Result.Fail(new Error(FailureCode.NotFound)
                    .WithMetadata("message", $"TourReview sa ID {id} nije pronađen"));
            }

            _context.TourReviews.Remove(tourReview);
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _context.TourReviews.AnyAsync(tr => tr.Id == id);
    }

    public async Task<Result<double?>> GetAverageRatingForTourAsync(long tourId)
    {
        try
        {
            var averageRating = await _context.TourReviews
                .Where(tr => tr.TourId == tourId && tr.Rating.HasValue)
                .AverageAsync(tr => (double?)tr.Rating);
            
            return Result.Ok(averageRating);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result<int>> GetReviewCountForTourAsync(long tourId)
    {
        try
        {
            var count = await _context.TourReviews
                .CountAsync(tr => tr.TourId == tourId);
            
            return Result.Ok(count);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }
}