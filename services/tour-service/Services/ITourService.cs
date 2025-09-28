using FluentResults;
using TourService.DTO;

namespace TourService.Services;

public interface ITourService
{
    Task<Result<TourDto>> CreateTourAsync(CreateTourRequestDto request, long authorId);
    Task<Result<List<TourDto>>> GetToursByAuthorAsync(long authorId);
    Task<Result<TourDto>> GetTourByIdAsync(long id);
    Task<Result<List<TourDto>>> GetAllToursAsync();
}