using FluentResults;
using TourService.DTO;

namespace TourService.Services;

public interface ITourService
{
    Task<Result<TourDto>> CreateTourAsync(CreateTourRequestDto request, long authorId);
    Task<Result<List<TourDto>>> GetToursByAuthorAsync(long authorId);
    Task<Result<TourDto>> GetTourByIdAsync(long id);
    Task<Result<List<TourDto>>> GetAllToursAsync();
    Task<Result<List<TourDto>>> GetPublicToursAsync();
    
    Task<Result<TourDto>> AddKeyPointAsync(long tourId, CreateTourKeyPointRequestDto keyPointDto, long authorId);
    Task<Result<TourDto>> AddTransportTimeAsync(long tourId, CreateTourTransportTimeRequestDto transportTimeDto, long authorId);
    Task<Result<TourDto>> PublishTourAsync(long tourId, long authorId);
    Task<Result<TourDto>> ArchiveTourAsync(long tourId, long authorId);
    Task<Result<TourDto>> ActivateTourAsync(long tourId, long authorId);
}