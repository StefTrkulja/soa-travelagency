using FluentResults;
using Microsoft.EntityFrameworkCore;
using TourService.Common;
using TourService.Database;
using TourService.Domain;
using TourService.Domain.RepositoryInterfaces;

namespace TourService.Repositories;

public class TourRepository : ITourRepository
{
    private readonly TourContext _context;

    public TourRepository(TourContext context)
    {
        _context = context;
    }

    public async Task<Result<Tour>> CreateAsync(Tour tour)
    {
        try
        {
            _context.Tours.Add(tour);
            await _context.SaveChangesAsync();
            return Result.Ok(tour);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result<Tour?>> GetByIdAsync(long id)
    {
        try
        {
            var tour = await _context.Tours
                .Include(t => t.TourTags)
                .ThenInclude(tt => tt.Tag)
                .Include(t => t.KeyPoints)
                .Include(t => t.TransportTimes)
                .Include(t => t.Reviews)
                .FirstOrDefaultAsync(t => t.Id == id);
            
            return Result.Ok(tour);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result<List<Tour>>> GetByAuthorIdAsync(long authorId)
    {
        try
        {
            var tours = await _context.Tours
                .Include(t => t.TourTags)
                .ThenInclude(tt => tt.Tag)
                .Include(t => t.KeyPoints)
                .Include(t => t.TransportTimes)
                .Include(t => t.Reviews)
                .Where(t => t.AuthorId == authorId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
            
            return Result.Ok(tours);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result<List<Tour>>> GetAllAsync()
    {
        try
        {
            var tours = await _context.Tours
                .Include(t => t.TourTags)
                .ThenInclude(tt => tt.Tag)
                .Include(t => t.KeyPoints)
                .Include(t => t.TransportTimes)
                .Include(t => t.Reviews)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
            
            return Result.Ok(tours);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result<List<Tour>>> GetPublishedToursAsync()
    {
        try
        {
            var tours = await _context.Tours
                .Include(t => t.TourTags)
                .ThenInclude(tt => tt.Tag)
                .Include(t => t.KeyPoints)
                .Include(t => t.TransportTimes)
                .Include(t => t.Reviews)
                .Where(t => t.Status == TourStatus.Published)
                .OrderByDescending(t => t.PublishedAt)
                .ToListAsync();
            
            return Result.Ok(tours);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result<Tour>> UpdateAsync(Tour tour)
    {
        try
        {
            tour.UpdatedAt = DateTime.UtcNow;
            _context.Tours.Update(tour);
            await _context.SaveChangesAsync();
            return Result.Ok(tour);
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
            var tour = await _context.Tours.FindAsync(id);
            if (tour == null)
            {
                return Result.Fail(new Error(FailureCode.NotFound)
                    .WithMetadata("message", $"Tour sa ID {id} nije pronađen"));
            }

            _context.Tours.Remove(tour);
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
        return await _context.Tours.AnyAsync(t => t.Id == id);
    }
}