using FluentResults;
using Microsoft.EntityFrameworkCore;
using TourService.Common;
using TourService.Database;
using TourService.Domain;
using TourService.Domain.RepositoryInterfaces;

namespace TourService.Repositories;

public class TagRepository : ITagRepository
{
    private readonly TourContext _context;

    public TagRepository(TourContext context)
    {
        _context = context;
    }

    public async Task<Result<Tag>> CreateAsync(Tag tag)
    {
        try
        {
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return Result.Ok(tag);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result<Tag?>> GetByNameAsync(string name)
    {
        try
        {
            var tag = await _context.Tags
                .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
            
            return Result.Ok(tag);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result<List<Tag>>> GetByNamesAsync(List<string> names)
    {
        try
        {
            var normalizedNames = names.Select(n => n.ToLower()).ToList();
            var tags = await _context.Tags
                .Where(t => normalizedNames.Contains(t.Name.ToLower()))
                .ToListAsync();
            
            return Result.Ok(tags);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result<List<Tag>>> GetAllAsync()
    {
        try
        {
            var tags = await _context.Tags
                .OrderBy(t => t.Name)
                .ToListAsync();
            
            return Result.Ok(tags);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result<Tag?>> GetByIdAsync(long id)
    {
        try
        {
            var tag = await _context.Tags.FindAsync(id);
            return Result.Ok(tag);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }
}