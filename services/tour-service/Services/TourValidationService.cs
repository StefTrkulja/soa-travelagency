using FluentResults;
using TourService.Common;
using TourService.Domain;

namespace TourService.Services;

public class TourValidationService : ITourValidationService
{
    private readonly ILogger<TourValidationService> _logger;

    public TourValidationService(ILogger<TourValidationService> logger)
    {
        _logger = logger;
    }

    public Task<Result> ValidateTourForPublishingAsync(Tour tour)
    {
        var errors = new List<IError>();

        // 1. Tour must have a name, description, at least one tag, at least two key points, and at least one transport time
        if (string.IsNullOrWhiteSpace(tour.Name))
        {
            errors.Add(new Error(FailureCode.ValidationError)
                .WithMetadata("message", "Naziv ture je obavezan"));
        }

        if (string.IsNullOrWhiteSpace(tour.Description))
        {
            errors.Add(new Error(FailureCode.ValidationError)
                .WithMetadata("message", "Opis ture je obavezan"));
        }

        if (tour.TourTags == null || !tour.TourTags.Any())
        {
            errors.Add(new Error(FailureCode.ValidationError)
                .WithMetadata("message", "Tura mora imati najmanje jedan tag"));
        }

        // 2. Tour must have at least two key points
        if (tour.KeyPoints == null || tour.KeyPoints.Count < 2)
        {
            errors.Add(new Error(FailureCode.ValidationError)
                .WithMetadata("message", "Tura mora imati najmanje dve ključne tačke"));
        }

        // 3. Tour must have at least one transport time
        if (tour.TransportTimes == null || !tour.TransportTimes.Any())
        {
            errors.Add(new Error(FailureCode.ValidationError)
                .WithMetadata("message", "Tura mora imati definisano bar jedno vreme prevoza"));
        }

        if (errors.Any())
        {
            return Task.FromResult(Result.Fail(errors));
        }

        return Task.FromResult(Result.Ok());
    }

    public async Task<bool> CanPublishTourAsync(Tour tour)
    {
        var validationResult = await ValidateTourForPublishingAsync(tour);
        return validationResult.IsSuccess;
    }

    public Task<bool> CanArchiveTourAsync(Tour tour)
    {
        // Archives can only be archived for published tours
        return Task.FromResult(tour.Status == TourStatus.Published);
    }

    public Task<bool> CanActivateTourAsync(Tour tour)
    {
        // Activate can only be activated for archived tours
        return Task.FromResult(tour.Status == TourStatus.Archived);
    }
}
