using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TourService.Controllers;

[ApiController]
public class ErrorsController : ControllerBase
{
    [Route("/error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Error()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = context?.Error;

        var (statusCode, message) = exception switch
        {
            ArgumentException => (400, "Neispravni parametri zahteva"),
            UnauthorizedAccessException => (401, "Neautorizovan pristup"),
            _ => (500, "Interna greška servera")
        };

        return Problem(statusCode: statusCode, title: message);
    }
}