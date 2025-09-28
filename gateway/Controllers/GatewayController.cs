using Gateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Gateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GatewayController : ControllerBase
{
    private readonly IServiceProxy _serviceProxy;
    private readonly ILogger<GatewayController> _logger;

    public GatewayController(IServiceProxy serviceProxy, ILogger<GatewayController> logger)
    {
        _serviceProxy = serviceProxy;
        _logger = logger;
    }

    // Stakeholders service endpoints
    [HttpPost("stakeholders/users/login")]
    public async Task<IActionResult> Login()
    {
        return await ForwardRequest("stakeholders", "api/users/login", HttpMethod.Post);
    }

    [HttpPost("stakeholders/users/register")]
    public async Task<IActionResult> Register()
    {
        return await ForwardRequest("stakeholders", "api/users/register", HttpMethod.Post);
    }

    [HttpGet("stakeholders/administrator/account")]
    [Authorize(Policy = "administratorPolicy")]
    public async Task<IActionResult> GetAccounts()
    {
        return await ForwardRequest("stakeholders", "api/administrator/account", HttpMethod.Get, includeAuth: true);
    }

    // Tours service endpoints
    [HttpPost("tours")]
    [Authorize(Policy = "authorPolicy")]
    public async Task<IActionResult> CreateTour()
    {
        return await ForwardRequest("tours", "api/tours", HttpMethod.Post, includeAuth: true);
    }

    [HttpGet("tours/my")]
    [Authorize(Policy = "authorPolicy")]
    public async Task<IActionResult> GetMyTours()
    {
        return await ForwardRequest("tours", "api/tours/my", HttpMethod.Get, includeAuth: true);
    }

    [HttpGet("tours/{id}")]
    [Authorize]
    public async Task<IActionResult> GetTour(long id)
    {
        return await ForwardRequest("tours", $"api/tours/{id}", HttpMethod.Get, includeAuth: true);
    }

    [HttpGet("tours")]
    [Authorize]
    public async Task<IActionResult> GetAllTours()
    {
        return await ForwardRequest("tours", "api/tours", HttpMethod.Get, includeAuth: true);
    }

    private async Task<IActionResult> ForwardRequest(string serviceName, string path, HttpMethod method, bool includeAuth = false)
    {
        try
        {
            HttpContent? content = null;
            
            if (method == HttpMethod.Post || method == HttpMethod.Put || method == HttpMethod.Patch)
            {
                Request.EnableBuffering();
                Request.Body.Position = 0;
                var body = await ReadRequestBodyAsync();
                if (!string.IsNullOrEmpty(body))
                {
                    content = new StringContent(body, Encoding.UTF8, "application/json");
                }
            }

            string? authToken = null;
            if (includeAuth)
            {
                authToken = ExtractTokenFromHeader();
            }

            var response = await _serviceProxy.ForwardRequestAsync(serviceName, path, method, content, authToken);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            
            return StatusCode((int)response.StatusCode, 
                string.IsNullOrEmpty(responseContent) ? null : responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing request for {ServiceName}/{Path}", serviceName, path);
            return StatusCode(500, new { message = "Gateway error occurred" });
        }
    }

    private async Task<string> ReadRequestBodyAsync()
    {
        using var reader = new StreamReader(Request.Body, leaveOpen: true);
        return await reader.ReadToEndAsync();
    }

    private string? ExtractTokenFromHeader()
    {
        var authHeader = Request.Headers.Authorization.FirstOrDefault();
        if (authHeader != null && authHeader.StartsWith("Bearer "))
        {
            return authHeader.Substring("Bearer ".Length);
        }
        return null;
    }
}