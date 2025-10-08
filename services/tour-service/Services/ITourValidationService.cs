using FluentResults;
using TourService.Domain;

namespace TourService.Services;

public interface ITourValidationService
{
    Task<Result> ValidateTourForPublishingAsync(Tour tour);
    Task<bool> CanPublishTourAsync(Tour tour);
    Task<bool> CanArchiveTourAsync(Tour tour);
    Task<bool> CanActivateTourAsync(Tour tour);
}
