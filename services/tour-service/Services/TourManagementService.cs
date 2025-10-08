using AutoMapper;
using FluentResults;
using TourService.Common;
using TourService.Database;
using TourService.Domain;
using TourService.Domain.RepositoryInterfaces;
using TourService.DTO;

namespace TourService.Services;

public class TourManagementService : ITourService
{
    private readonly ITourRepository _tourRepository;
    private readonly ITagRepository _tagRepository;
    private readonly TourContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<TourManagementService> _logger;
    private readonly ITourValidationService _validationService;

    public TourManagementService(
        ITourRepository tourRepository,
        ITagRepository tagRepository,
        TourContext context,
        IMapper mapper,
        ILogger<TourManagementService> logger,
        ITourValidationService validationService)
    {
        _tourRepository = tourRepository;
        _tagRepository = tagRepository;
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _validationService = validationService;
    }

    public async Task<Result<TourDto>> CreateTourAsync(CreateTourRequestDto request, long authorId)
    {
        try
        {
            _logger.LogInformation("Kreiranje nove ture za autora {AuthorId}", authorId);

            if (!Enum.IsDefined(typeof(TourDifficulty), request.Difficulty))
            {
                return Result.Fail(new Error(FailureCode.ValidationError)
                    .WithMetadata("message", "Neispravna vrednost za težinu ture"));
            }

            var tour = _mapper.Map<Tour>(request);
            tour.AuthorId = authorId;

            var createTourResult = await _tourRepository.CreateAsync(tour);
            if (createTourResult.IsFailed)
            {
                return Result.Fail(createTourResult.Errors);
            }

            var createdTour = createTourResult.Value;

            var tagsResult = await ProcessTagsAsync(request.Tags, createdTour.Id);
            if (tagsResult.IsFailed)
            {
                await _tourRepository.DeleteAsync(createdTour.Id);
                return Result.Fail(tagsResult.Errors);
            }

            // Process key points
            if (request.KeyPoints != null && request.KeyPoints.Any())
            {
                var keyPointsResult = await ProcessKeyPointsAsync(request.KeyPoints, createdTour.Id);
                if (keyPointsResult.IsFailed)
                {
                    await _tourRepository.DeleteAsync(createdTour.Id);
                    return Result.Fail(keyPointsResult.Errors);
                }
            }

            // Process transport times
            if (request.TransportTimes != null && request.TransportTimes.Any())
            {
                var transportTimesResult = await ProcessTransportTimesAsync(request.TransportTimes, createdTour.Id);
                if (transportTimesResult.IsFailed)
                {
                    await _tourRepository.DeleteAsync(createdTour.Id);
                    return Result.Fail(transportTimesResult.Errors);
                }
            }

            var completeTourResult = await _tourRepository.GetByIdAsync(createdTour.Id);
            if (completeTourResult.IsFailed || completeTourResult.Value == null)
            {
                return Result.Fail(new Error(FailureCode.DatabaseError)
                    .WithMetadata("message", "Greška pri učitavanju kreiane ture"));
            }

            var tourDto = _mapper.Map<TourDto>(completeTourResult.Value);
            
            _logger.LogInformation("Uspešno kreirana tura sa ID {TourId}", createdTour.Id);
            return Result.Ok(tourDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Greška pri kreiranju ture za autora {AuthorId}", authorId);
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result<List<TourDto>>> GetToursByAuthorAsync(long authorId)
    {
        try
        {
            _logger.LogInformation("Preuzimanje tura za autora {AuthorId}", authorId);

            var toursResult = await _tourRepository.GetByAuthorIdAsync(authorId);
            if (toursResult.IsFailed)
            {
                return Result.Fail(toursResult.Errors);
            }

            var tourDtos = _mapper.Map<List<TourDto>>(toursResult.Value);
            
            _logger.LogInformation("Pronađeno {Count} tura za autora {AuthorId}", 
                tourDtos.Count, authorId);
            
            return Result.Ok(tourDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Greška pri preuzimanju tura za autora {AuthorId}", authorId);
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result<TourDto>> GetTourByIdAsync(long id)
    {
        try
        {
            var tourResult = await _tourRepository.GetByIdAsync(id);
            if (tourResult.IsFailed)
            {
                return Result.Fail(tourResult.Errors);
            }

            if (tourResult.Value == null)
            {
                return Result.Fail(new Error(FailureCode.NotFound)
                    .WithMetadata("message", $"Tura sa ID {id} nije pronađena"));
            }

            var tourDto = _mapper.Map<TourDto>(tourResult.Value);
            return Result.Ok(tourDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Greška pri preuzimanju ture sa ID {TourId}", id);
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result<List<TourDto>>> GetAllToursAsync()
    {
        try
        {
            var toursResult = await _tourRepository.GetAllAsync();
            if (toursResult.IsFailed)
            {
                return Result.Fail(toursResult.Errors);
            }

            var tourDtos = _mapper.Map<List<TourDto>>(toursResult.Value);
            return Result.Ok(tourDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Greška pri preuzimanju svih tura");
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result<List<TourDto>>> GetPublicToursAsync()
    {
        try
        {
            var toursResult = await _tourRepository.GetPublishedToursAsync();
            if (toursResult.IsFailed)
            {
                return Result.Fail(toursResult.Errors);
            }

            var tourDtos = _mapper.Map<List<TourDto>>(toursResult.Value);
            
            _logger.LogInformation("Pronađeno {Count} javnih tura", tourDtos.Count);
            
            return Result.Ok(tourDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Greška pri preuzimanju javnih tura");
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    private async Task<Result> ProcessTagsAsync(List<string> tagNames, long tourId)
    {
        try
        {
            var existingTagsResult = await _tagRepository.GetByNamesAsync(tagNames);
            if (existingTagsResult.IsFailed)
            {
                return Result.Fail(existingTagsResult.Errors);
            }

            var existingTags = existingTagsResult.Value;
            var existingTagNames = existingTags.Select(t => t.Name.ToLower()).ToList();

            var newTagNames = tagNames
                .Where(name => !existingTagNames.Contains(name.ToLower()))
                .ToList();

            var allTags = new List<Tag>(existingTags);

            foreach (var tagName in newTagNames)
            {
                var newTag = new Tag { Name = tagName.Trim() };
                var createTagResult = await _tagRepository.CreateAsync(newTag);
                if (createTagResult.IsFailed)
                {
                    return Result.Fail(createTagResult.Errors);
                }
                allTags.Add(createTagResult.Value);
            }

            foreach (var tag in allTags)
            {
                var tourTag = new TourTag
                {
                    TourId = tourId,
                    TagId = tag.Id
                };
                _context.TourTags.Add(tourTag);
            }

            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result<TourDto>> AddKeyPointAsync(long tourId, CreateTourKeyPointRequestDto keyPointDto, long authorId)
    {
        try
        {
            var tourResult = await _tourRepository.GetByIdAsync(tourId);
            if (tourResult.IsFailed || tourResult.Value == null)
            {
                return Result.Fail(new Error(FailureCode.NotFound)
                    .WithMetadata("message", $"Tura sa ID {tourId} nije pronađena"));
            }

            var tour = tourResult.Value;
            if (tour.AuthorId != authorId)
            {
                return Result.Fail(new Error(FailureCode.Forbidden)
                    .WithMetadata("message", "Nemate dozvolu za izmenu ove ture"));
            }

            var keyPoint = _mapper.Map<TourKeyPoint>(keyPointDto);
            keyPoint.TourId = tourId;
            keyPoint.Order = tour.KeyPoints.Count + 1;

            _context.TourKeyPoints.Add(keyPoint);
            await _context.SaveChangesAsync();

            var updatedTourResult = await _tourRepository.GetByIdAsync(tourId);
            if (updatedTourResult.IsFailed || updatedTourResult.Value == null)
            {
                return Result.Fail(new Error(FailureCode.DatabaseError)
                    .WithMetadata("message", "Greška pri učitavanju ažurirane ture"));
            }

            var tourDto = _mapper.Map<TourDto>(updatedTourResult.Value);
            return Result.Ok(tourDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Greška pri dodavanju ključne tačke u turu {TourId}", tourId);
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result<TourDto>> AddTransportTimeAsync(long tourId, CreateTourTransportTimeRequestDto transportTimeDto, long authorId)
    {
        try
        {
            var tourResult = await _tourRepository.GetByIdAsync(tourId);
            if (tourResult.IsFailed || tourResult.Value == null)
            {
                return Result.Fail(new Error(FailureCode.NotFound)
                    .WithMetadata("message", $"Tura sa ID {tourId} nije pronađena"));
            }

            var tour = tourResult.Value;
            if (tour.AuthorId != authorId)
            {
                return Result.Fail(new Error(FailureCode.Forbidden)
                    .WithMetadata("message", "Nemate dozvolu za izmenu ove ture"));
            }

            // Check if transport type already exists
            var transportType = (TransportType)transportTimeDto.TransportType;
            if (tour.TransportTimes.Any(tt => tt.TransportType == transportType))
            {
                return Result.Fail(new Error(FailureCode.ValidationError)
                    .WithMetadata("message", $"Vreme prevoza za {transportType} već postoji"));
            }

            var transportTime = _mapper.Map<TourTransportTime>(transportTimeDto);
            transportTime.TourId = tourId;

            _context.TourTransportTimes.Add(transportTime);
            await _context.SaveChangesAsync();

            var updatedTourResult = await _tourRepository.GetByIdAsync(tourId);
            if (updatedTourResult.IsFailed || updatedTourResult.Value == null)
            {
                return Result.Fail(new Error(FailureCode.DatabaseError)
                    .WithMetadata("message", "Greška pri učitavanju ažurirane ture"));
            }

            var tourDto = _mapper.Map<TourDto>(updatedTourResult.Value);
            return Result.Ok(tourDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Greška pri dodavanju vremena prevoza u turu {TourId}", tourId);
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result<TourDto>> PublishTourAsync(long tourId, long authorId)
    {
        try
        {
            var tourResult = await _tourRepository.GetByIdAsync(tourId);
            if (tourResult.IsFailed || tourResult.Value == null)
            {
                return Result.Fail(new Error(FailureCode.NotFound)
                    .WithMetadata("message", $"Tura sa ID {tourId} nije pronađena"));
            }

            var tour = tourResult.Value;
            if (tour.AuthorId != authorId)
            {
                return Result.Fail(new Error(FailureCode.Forbidden)
                    .WithMetadata("message", "Nemate dozvolu za izmenu ove ture"));
            }

            var canPublish = await _validationService.CanPublishTourAsync(tour);
            if (!canPublish)
            {
                return Result.Fail(new Error(FailureCode.ValidationError)
                    .WithMetadata("message", "Tura ne ispunjava uslove za objavljivanje"));
            }

            tour.Status = TourStatus.Published;
            tour.PublishedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var tourDto = _mapper.Map<TourDto>(tour);
            return Result.Ok(tourDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Greška pri objavljivanju ture {TourId}", tourId);
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result<TourDto>> ArchiveTourAsync(long tourId, long authorId)
    {
        try
        {
            var tourResult = await _tourRepository.GetByIdAsync(tourId);
            if (tourResult.IsFailed || tourResult.Value == null)
            {
                return Result.Fail(new Error(FailureCode.NotFound)
                    .WithMetadata("message", $"Tura sa ID {tourId} nije pronađena"));
            }

            var tour = tourResult.Value;
            if (tour.AuthorId != authorId)
            {
                return Result.Fail(new Error(FailureCode.Forbidden)
                    .WithMetadata("message", "Nemate dozvolu za izmenu ove ture"));
            }

            var canArchive = await _validationService.CanArchiveTourAsync(tour);
            if (!canArchive)
            {
                return Result.Fail(new Error(FailureCode.ValidationError)
                    .WithMetadata("message", "Samo objavljene ture se mogu arhivirati"));
            }

            tour.Status = TourStatus.Archived;
            tour.ArchivedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var tourDto = _mapper.Map<TourDto>(tour);
            return Result.Ok(tourDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Greška pri arhiviranju ture {TourId}", tourId);
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    public async Task<Result<TourDto>> ActivateTourAsync(long tourId, long authorId)
    {
        try
        {
            var tourResult = await _tourRepository.GetByIdAsync(tourId);
            if (tourResult.IsFailed || tourResult.Value == null)
            {
                return Result.Fail(new Error(FailureCode.NotFound)
                    .WithMetadata("message", $"Tura sa ID {tourId} nije pronađena"));
            }

            var tour = tourResult.Value;
            if (tour.AuthorId != authorId)
            {
                return Result.Fail(new Error(FailureCode.Forbidden)
                    .WithMetadata("message", "Nemate dozvolu za izmenu ove ture"));
            }

            var canActivate = await _validationService.CanActivateTourAsync(tour);
            if (!canActivate)
            {
                return Result.Fail(new Error(FailureCode.ValidationError)
                    .WithMetadata("message", "Samo arhivirane ture se mogu aktivirati"));
            }

            tour.Status = TourStatus.Published;
            tour.ArchivedAt = null;
            await _context.SaveChangesAsync();

            var tourDto = _mapper.Map<TourDto>(tour);
            return Result.Ok(tourDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Greška pri aktiviranju ture {TourId}", tourId);
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    private async Task<Result> ProcessKeyPointsAsync(List<CreateTourKeyPointRequestDto> keyPoints, long tourId)
    {
        try
        {
            for (int i = 0; i < keyPoints.Count; i++)
            {
                var keyPoint = _mapper.Map<TourKeyPoint>(keyPoints[i]);
                keyPoint.TourId = tourId;
                keyPoint.Order = i + 1;
                _context.TourKeyPoints.Add(keyPoint);
            }

            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }

    private async Task<Result> ProcessTransportTimesAsync(List<CreateTourTransportTimeRequestDto> transportTimes, long tourId)
    {
        try
        {
            foreach (var transportTimeDto in transportTimes)
            {
                var transportTime = _mapper.Map<TourTransportTime>(transportTimeDto);
                transportTime.TourId = tourId;
                _context.TourTransportTimes.Add(transportTime);
            }

            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error(FailureCode.DatabaseError)
                .WithMetadata("exception", ex.Message));
        }
    }
}