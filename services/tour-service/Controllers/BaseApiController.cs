using FluentResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TourService.Common;

namespace TourService.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{

    protected long GetUserId()
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Korisnik nije autentifikovan ili nema valjan ID");
        }
        return userId;
    }

    protected string GetUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }

    protected string GetUsername()
    {
        return User.FindFirst("username")?.Value ?? string.Empty;
    }

    protected ActionResult CreateResponse<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        var error = result.Errors.FirstOrDefault();
        if (error == null)
        {
            return StatusCode(500, new { message = "Neočekivana greška" });
        }

        return error.Metadata.TryGetValue("reason", out var reasonObj) && reasonObj is string reason
            ? reason switch
            {
                FailureCode.NotFound => NotFound(new { message = GetErrorMessage(error) }),
                FailureCode.ValidationError => BadRequest(new { message = GetErrorMessage(error) }),
                FailureCode.Unauthorized => Unauthorized(new { message = GetErrorMessage(error) }),
                FailureCode.Forbidden => Forbid(GetErrorMessage(error)),
                FailureCode.DuplicateResource => Conflict(new { message = GetErrorMessage(error) }),
                _ => StatusCode(500, new { message = GetErrorMessage(error) })
            }
            : StatusCode(500, new { message = GetErrorMessage(error) });
    }

    protected ActionResult CreateResponse(Result result)
    {
        if (result.IsSuccess)
        {
            return Ok();
        }

        var error = result.Errors.FirstOrDefault();
        if (error == null)
        {
            return StatusCode(500, new { message = "Neočekivana greška" });
        }

        return error.Metadata.TryGetValue("reason", out var reasonObj) && reasonObj is string reason
            ? reason switch
            {
                FailureCode.NotFound => NotFound(new { message = GetErrorMessage(error) }),
                FailureCode.ValidationError => BadRequest(new { message = GetErrorMessage(error) }),
                FailureCode.Unauthorized => Unauthorized(new { message = GetErrorMessage(error) }),
                FailureCode.Forbidden => Forbid(GetErrorMessage(error)),
                FailureCode.DuplicateResource => Conflict(new { message = GetErrorMessage(error) }),
                _ => StatusCode(500, new { message = GetErrorMessage(error) })
            }
            : StatusCode(500, new { message = GetErrorMessage(error) });
    }

    private static string GetErrorMessage(IError error)
    {
        return error.Metadata.TryGetValue("message", out var messageObj) && messageObj is string message 
            ? message 
            : error.Message;
    }
}