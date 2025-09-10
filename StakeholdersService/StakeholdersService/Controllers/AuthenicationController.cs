using StakeholdersService.DTO;
using StakeholdersService;
using Microsoft.AspNetCore.Mvc;

using StakeholdersService.Services; 
namespace StakeholdersService.Controllers;

[Route("api/users")]
public class AuthenticationController : BaseApiController
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("register")]
    public ActionResult<AuthenticationTokensDto> RegisterUser([FromBody] AccountRegistrationDto account)
    {
        var result = _authenticationService.RegisterUser(account);
        return CreateResponse(result);
    }

    [HttpPost("login")]
    public ActionResult<AuthenticationTokensDto> Login([FromBody] CredentialsDto credentials)
    {
        var result = _authenticationService.Login(credentials);
        return CreateResponse(result);
    }
}