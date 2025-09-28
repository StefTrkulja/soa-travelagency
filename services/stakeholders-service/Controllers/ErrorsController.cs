using Microsoft.AspNetCore.Mvc;

namespace StakeholdersService.Controllers;

[ApiController]
public class ErrorsController : ControllerBase
{
    [HttpGet]
    [Route("/error")]
    public IActionResult HandleErrors() => Problem();
}