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

    /// <summary>
    /// Kreira novu turu. Dostupno samo autorima.
    /// </summary>
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

    /// <summary>
    /// Preuzima sve ture trenutnog autora. Dostupno samo autorima.
    /// </summary>
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

    /// <summary>
    /// Preuzima turu po ID-ju. Dostupno svim autentifikovanim korisnicima.
    /// </summary>
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

    /// <summary>
    /// Preuzima sve ture. Dostupno svim autentifikovanim korisnicima.
    /// </summary>
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
}