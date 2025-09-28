using FluentResults;
using TourService.Domain;

namespace TourService.Domain.RepositoryInterfaces;

public interface ITagRepository
{
    Task<Result<Tag>> CreateAsync(Tag tag);
    Task<Result<Tag?>> GetByNameAsync(string name);
    Task<Result<List<Tag>>> GetByNamesAsync(List<string> names);
    Task<Result<List<Tag>>> GetAllAsync();
    Task<Result<Tag?>> GetByIdAsync(long id);
}