using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourService.DTO;
using TourService.Services;

namespace TourService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ToursController : BaseApiController
{
    private readonly ITourService _tourService;
    private readonly ILogger<ToursController> _logger;

    public ToursController(ITourService tourService, ILogger<ToursController> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Policy = "authorPolicy")]
    public async Task<ActionResult<TourDto>> CreateTour(CreateTourRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authorId = GetUserId();
            var result = await _tourService.CreateTourAsync(request, authorId);
            
            return CreateResponse(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Neautorizovani pokušaj kreiranja ture");
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Greška pri kreiranju ture");
            return StatusCode(500, new { message = "Interna greška servera" });
        }
    }

    [HttpGet("my")]
    [Authorize(Policy = "authorPolicy")]
    public async Task<ActionResult<List<TourDto>>> GetMyTours()
    {
        try
        {
            var authorId = GetUserId();
            var result = await _tourService.GetToursByAuthorAsync(authorId);
            
            return CreateResponse(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Neautorizovani pokušaj preuzimanja tura");
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Greška pri preuzimanju tura autora");
            return StatusCode(500, new { message = "Interna greška servera" });
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<TourDto>> GetTour(long id)
    {
        try
        {
            var result = await _tourService.GetTourByIdAsync(id);
            return CreateResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Greška pri preuzimanju ture sa ID {TourId}", id);
            return StatusCode(500, new { message = "Interna greška servera" });
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<TourDto>>> GetAllTours()
    {
        try
        {
            var result = await _tourService.GetAllToursAsync();
            return CreateResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Greška pri preuzimanju svih tura");
            return StatusCode(500, new { message = "Interna greška servera" });
        }
    }

    [HttpGet("public")]
    [Authorize(Policy = "touristPolicy")]
    public async Task<ActionResult<List<TourDto>>> GetPublicTours()
    {
        try
        {
            var result = await _tourService.GetPublicToursAsync();
            return CreateResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Greška pri preuzimanju javnih tura");
            return StatusCode(500, new { message = "Interna greška servera" });
        }
    }

    [HttpPost("{tourId}/key-points")]
    [Authorize(Policy = "authorPolicy")]
    public async Task<ActionResult<TourDto>> AddKeyPoint(long tourId, CreateTourKeyPointRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authorId = GetUserId();
            var result = await _tourService.AddKeyPointAsync(tourId, request, authorId);
            
            return CreateResponse(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Neautorizovani pokušaj dodavanja ključne tačke");
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Greška pri dodavanju ključne tačke u turu {TourId}", tourId);
            return StatusCode(500, new { message = "Interna greška servera" });
        }
    }

    [HttpPost("{tourId}/transport-times")]
    [Authorize(Policy = "authorPolicy")]
    public async Task<ActionResult<TourDto>> AddTransportTime(long tourId, CreateTourTransportTimeRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authorId = GetUserId();
            var result = await _tourService.AddTransportTimeAsync(tourId, request, authorId);
            
            return CreateResponse(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Neautorizovani pokušaj dodavanja vremena prevoza");
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Greška pri dodavanju vremena prevoza u turu {TourId}", tourId);
            return StatusCode(500, new { message = "Interna greška servera" });
        }
    }

    [HttpPost("{tourId}/publish")]
    [Authorize(Policy = "authorPolicy")]
    public async Task<ActionResult<TourDto>> PublishTour(long tourId)
    {
        try
        {
            var authorId = GetUserId();
            var result = await _tourService.PublishTourAsync(tourId, authorId);
            
            return CreateResponse(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Neautorizovani pokušaj objavljivanja ture");
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Greška pri objavljivanju ture {TourId}", tourId);
            return StatusCode(500, new { message = "Interna greška servera" });
        }
    }

    [HttpPost("{tourId}/archive")]
    [Authorize(Policy = "authorPolicy")]
    public async Task<ActionResult<TourDto>> ArchiveTour(long tourId)
    {
        try
        {
            var authorId = GetUserId();
            var result = await _tourService.ArchiveTourAsync(tourId, authorId);
            
            return CreateResponse(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Neautorizovani pokušaj arhiviranja ture");
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Greška pri arhiviranju ture {TourId}", tourId);
            return StatusCode(500, new { message = "Interna greška servera" });
        }
    }

    [HttpPost("{tourId}/activate")]
    [Authorize(Policy = "authorPolicy")]
    public async Task<ActionResult<TourDto>> ActivateTour(long tourId)
    {
        try
        {
            var authorId = GetUserId();
            var result = await _tourService.ActivateTourAsync(tourId, authorId);
            
            return CreateResponse(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Neautorizovani pokušaj aktiviranja ture");
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Greška pri aktiviranju ture {TourId}", tourId);
            return StatusCode(500, new { message = "Interna greška servera" });
        }
    }
}