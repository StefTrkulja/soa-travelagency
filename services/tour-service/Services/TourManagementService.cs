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

    public TourManagementService(
        ITourRepository tourRepository,
        ITagRepository tagRepository,
        TourContext context,
        IMapper mapper,
        ILogger<TourManagementService> logger)
    {
        _tourRepository = tourRepository;
        _tagRepository = tagRepository;
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<TourDto>> CreateTourAsync(CreateTourRequestDto request, long authorId)
    {
        try
        {
            _logger.LogInformation("Kreiranje nove ture za autora {AuthorId}", authorId);

            // Validate difficulty enum
            if (!Enum.IsDefined(typeof(TourDifficulty), request.Difficulty))
            {
                return Result.Fail(new Error(FailureCode.ValidationError)
                    .WithMetadata("message", "Neispravna vrednost za težinu ture"));
            }

            // Map DTO to domain entity
            var tour = _mapper.Map<Tour>(request);
            tour.AuthorId = authorId;

            // Create tour first
            var createTourResult = await _tourRepository.CreateAsync(tour);
            if (createTourResult.IsFailed)
            {
                return Result.Fail(createTourResult.Errors);
            }

            var createdTour = createTourResult.Value;

            // Handle tags
            var tagsResult = await ProcessTagsAsync(request.Tags, createdTour.Id);
            if (tagsResult.IsFailed)
            {
                // Rollback tour creation if tag processing fails
                await _tourRepository.DeleteAsync(createdTour.Id);
                return Result.Fail(tagsResult.Errors);
            }

            // Retrieve the complete tour with tags
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

    private async Task<Result> ProcessTagsAsync(List<string> tagNames, long tourId)
    {
        try
        {
            // Get existing tags
            var existingTagsResult = await _tagRepository.GetByNamesAsync(tagNames);
            if (existingTagsResult.IsFailed)
            {
                return Result.Fail(existingTagsResult.Errors);
            }

            var existingTags = existingTagsResult.Value;
            var existingTagNames = existingTags.Select(t => t.Name.ToLower()).ToList();

            // Create new tags that don't exist
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

            // Create TourTag relationships
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
}