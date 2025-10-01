using StakeholdersService.DTO;
using StakeholdersService;
using Microsoft.AspNetCore.Mvc;
using StakeholdersService.Services;

namespace StakeholdersService.Controllers;

[Route("api/users")]
public class AuthenticationController : BaseApiController
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(IAuthenticationService authenticationService, ILogger<AuthenticationController> logger)
    {
        _authenticationService = authenticationService;
        _logger = logger;
    }

    [HttpPost("register")]
    public ActionResult<AuthenticationTokensDto> RegisterUser([FromBody] AccountRegistrationDto account)
    {
        _logger.LogInformation("User registration attempt for email: {Email}, username: {Username}", 
            account.Email, account.Username);
        
        var result = _authenticationService.RegisterUser(account);
        
        if (result.IsSuccess)
        {
            _logger.LogInformation("User registration successful for email: {Email}, username: {Username}", 
                account.Email, account.Username);
        }
        else
        {
            _logger.LogWarning("User registration failed for email: {Email}, username: {Username}, Errors: {Errors}", 
                account.Email, account.Username, string.Join(", ", result.Errors.Select(e => e.Message)));
        }
        
        return CreateResponse(result);
    }

    [HttpPost("login")]
    public ActionResult<AuthenticationTokensDto> Login([FromBody] CredentialsDto credentials)
    {
        _logger.LogInformation("Login attempt for username: {Username}", credentials.Username);
        
        var result = _authenticationService.Login(credentials);
        
        if (result.IsSuccess)
        {
            _logger.LogInformation("Login successful for username: {Username}", credentials.Username);
        }
        else
        {
            _logger.LogWarning("Login failed for username: {Username}, Errors: {Errors}", 
                credentials.Username, string.Join(", ", result.Errors.Select(e => e.Message)));
        }
        
        return CreateResponse(result);
    }
}