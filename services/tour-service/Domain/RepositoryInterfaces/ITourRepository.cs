using FluentResults;
using TourService.Domain;

namespace TourService.Domain.RepositoryInterfaces;

public interface ITourRepository
{
    Task<Result<Tour>> CreateAsync(Tour tour);
    Task<Result<Tour?>> GetByIdAsync(long id);
    Task<Result<List<Tour>>> GetByAuthorIdAsync(long authorId);
    Task<Result<List<Tour>>> GetAllAsync();
    Task<Result<List<Tour>>> GetPublishedToursAsync();
    Task<Result<Tour>> UpdateAsync(Tour tour);
    Task<Result> DeleteAsync(long id);
    Task<bool> ExistsAsync(long id);
}