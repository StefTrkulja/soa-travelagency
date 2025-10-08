using Gateway.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

    [HttpPut("stakeholders/administrator/account/{userId}/block")]
    [Authorize(Policy = "administratorPolicy")]
    public async Task<IActionResult> BlockUser(long userId)
    {
        return await ForwardRequest("stakeholders", $"api/administrator/account/{userId}/block", HttpMethod.Put, includeAuth: true);
    }

    [HttpPut("stakeholders/administrator/account/{userId}/unblock")]
    [Authorize(Policy = "administratorPolicy")]
    public async Task<IActionResult> UnblockUser(long userId)
    {
        return await ForwardRequest("stakeholders", $"api/administrator/account/{userId}/unblock", HttpMethod.Put, includeAuth: true);
    }

    [HttpGet("stakeholders/profile")]
    [Authorize]
    public async Task<IActionResult> GetMyProfile()
    {
        return await ForwardRequest("stakeholders", "api/profile", HttpMethod.Get, includeAuth: true);
    }

    [HttpGet("stakeholders/profile/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetUserProfile(long userId)
    {
        return await ForwardRequest("stakeholders", $"api/profile/{userId}", HttpMethod.Get, includeAuth: true);
    }

    [HttpGet("stakeholders/profile/all")]
    [Authorize]
    public async Task<IActionResult> GetAllUsers()
    {
        return await ForwardRequest("stakeholders", "api/profile/all", HttpMethod.Get, includeAuth: true);
    }

    [HttpPut("stakeholders/profile")]
    [Authorize]
    public async Task<IActionResult> UpdateMyProfile()
    {
        return await ForwardRequest("stakeholders", "api/profile", HttpMethod.Put, includeAuth: true);
    }

    [HttpPost("tours")]
    [Authorize(Policy = "authorPolicy")]
    public async Task<IActionResult> CreateTour()
    {
        return await ForwardRequest("tours", "api/tours", HttpMethod.Post, includeAuth: true);
    }

    // Blog service endpoints
    [HttpPost("blogs")]
    [Authorize(Policy = "authorPolicy")]
    public async Task<IActionResult> CreateBlog()
    {
        return await ForwardMultipartToBlogs("api/blogs", HttpMethod.Post, includeAuth: true, addUserIdHeader: true);
    }

    [HttpGet("blogs")]
    public async Task<IActionResult> GetAllBlogs()
    {
        return await ForwardRequest("blogs", "api/blogs", HttpMethod.Get);
    }

    [HttpPost("blogs/following")]
    [Authorize]
    public async Task<IActionResult> GetFollowingBlogs()
    {
        return await ForwardRequest("blogs", "api/blogs/following", HttpMethod.Post, includeAuth: true);
    }

    [HttpGet("blogs/my")]
    [Authorize]
    public async Task<IActionResult> GetMyBlogs()
    {
        return await ForwardRequestWithUserId("blogs", "api/blogs/my", HttpMethod.Get, includeAuth: true);
    }

    [HttpGet("blogs/{id}")]
    public async Task<IActionResult> GetBlog(long id)
    {
        return await ForwardRequest("blogs", $"api/blogs/{id}", HttpMethod.Get);
    }

    [HttpPut("blogs/{id}")]
    [Authorize(Policy = "authorPolicy")]
    public async Task<IActionResult> UpdateBlog(long id)
    {
        return await ForwardMultipartToBlogs($"api/blogs/{id}", HttpMethod.Put, includeAuth: true, addUserIdHeader: true);
    }

    [HttpDelete("blogs/{id}")]
    [Authorize(Policy = "authorPolicy")]
    public async Task<IActionResult> DeleteBlog(long id)
    {
        return await ForwardRequestWithUserId("blogs", $"api/blogs/{id}", HttpMethod.Delete, includeAuth: true);
    }

    // Follower service endpoints
    [HttpPost("followers/{id:long}/follow")]
    [Authorize]
    public async Task<IActionResult> FollowUser(long id)
    {
        return await ForwardFollowRequest("followers", "api/followers/follow", HttpMethod.Post, id, includeAuth: true);
    }

    [HttpPost("followers/{id:long}/unfollow")]
    [Authorize]
    public async Task<IActionResult> UnfollowUser(long id)
    {
        return await ForwardFollowRequest("followers", "api/followers/unfollow", HttpMethod.Post, id, includeAuth: true);
    }

    [HttpGet("followers/{id:long}/following")]
    [Authorize]
    public async Task<IActionResult> GetFollowing(long id)
    {
        return await ForwardRequest("followers", $"api/followers/{id}/following", HttpMethod.Get, includeAuth: true);
    }

    [HttpGet("followers/{id:long}/followers")]
    [Authorize]
    public async Task<IActionResult> GetFollowers(long id)
    {
        return await ForwardRequest("followers", $"api/followers/{id}/followers", HttpMethod.Get, includeAuth: true);
    }

    [HttpGet("followers/{id:long}/recommendations")]
    [Authorize]
    public async Task<IActionResult> GetRecommendations(long id, [FromQuery] int limit = 10)
    {
        return await ForwardRequest("followers", $"api/followers/{id}/recommendations?limit={limit}", HttpMethod.Get, includeAuth: true);
    }

    [HttpGet("followers/check")]
    [Authorize]
    public async Task<IActionResult> CheckFollowing([FromQuery] long followerId, [FromQuery] long followedId)
    {
        return await ForwardRequest("followers", $"api/followers/check?followerId={followerId}&followedId={followedId}", HttpMethod.Get, includeAuth: true);
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

    [HttpGet("tours/public")]
    [Authorize(Policy = "touristPolicy")]
    public async Task<IActionResult> GetPublicTours()
    {
        return await ForwardRequest("tours", "api/tours/public", HttpMethod.Get, includeAuth: true);
    }

    [HttpPost("tours/{tourId}/publish")]
    [Authorize(Policy = "authorPolicy")]
    public async Task<IActionResult> PublishTour(long tourId)
    {
        return await ForwardRequest("tours", $"api/tours/{tourId}/publish", HttpMethod.Post, includeAuth: true);
    }

    [HttpPost("tours/{tourId}/archive")]
    [Authorize(Policy = "authorPolicy")]
    public async Task<IActionResult> ArchiveTour(long tourId)
    {
        return await ForwardRequest("tours", $"api/tours/{tourId}/archive", HttpMethod.Post, includeAuth: true);
    }

    [HttpPost("tours/{tourId}/activate")]
    [Authorize(Policy = "authorPolicy")]
    public async Task<IActionResult> ActivateTour(long tourId)
    {
        return await ForwardRequest("tours", $"api/tours/{tourId}/activate", HttpMethod.Post, includeAuth: true);
    }

    [HttpPost("tours/{tourId}/key-points")]
    [Authorize(Policy = "authorPolicy")]
    public async Task<IActionResult> AddKeyPoint(long tourId)
    {
        return await ForwardRequest("tours", $"api/tours/{tourId}/key-points", HttpMethod.Post, includeAuth: true);
    }

    [HttpPost("tours/{tourId}/transport-times")]
    [Authorize(Policy = "authorPolicy")]
    public async Task<IActionResult> AddTransportTime(long tourId)
    {
        return await ForwardRequest("tours", $"api/tours/{tourId}/transport-times", HttpMethod.Post, includeAuth: true);
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

    private async Task<IActionResult> ForwardRequestWithUserId(string serviceName, string path, HttpMethod method, bool includeAuth = false)
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
            IDictionary<string, string>? extraHeaders = null;
            if (includeAuth)
            {
                authToken = ExtractTokenFromHeader();
                if (!string.IsNullOrEmpty(authToken))
                {
                    var userId = ExtractUserIdFromJwt(authToken);
                    extraHeaders = new Dictionary<string, string> { { "X-User-Id", userId.ToString() } };
                }
            }

            var response = await _serviceProxy.ForwardRequestAsync(serviceName, path, method, content, authToken, extraHeaders);
            
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

    private async Task<IActionResult> ForwardMultipartToBlogs(string path, HttpMethod method, bool includeAuth = false, bool addUserIdHeader = false)
    {
        try
        {
            MultipartFormDataContent? form = null;
            if (Request.HasFormContentType)
            {
                var formCollection = await Request.ReadFormAsync();
                form = new MultipartFormDataContent();
                foreach (var formField in formCollection)
                {
                    if (formField.Key == "payload")
                    {
                        var jsonPart = new StringContent(formField.Value, Encoding.UTF8, "application/json");
                        jsonPart.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        form.Add(jsonPart, formField.Key);
                    }
                    else
                    {
                        form.Add(new StringContent(formField.Value), formField.Key);
                    }
                }
                foreach (var file in formCollection.Files)
                {
                    var streamContent = new StreamContent(file.OpenReadStream());
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    form.Add(streamContent, file.Name, file.FileName);
                }
            }

            string? authToken = null;
            if (includeAuth)
            {
                authToken = ExtractTokenFromHeader();
            }

            IDictionary<string, string>? extraHeaders = null;
            if (addUserIdHeader && !string.IsNullOrEmpty(authToken))
            {
                var userId = ExtractUserIdFromJwt(authToken);
                extraHeaders = new Dictionary<string, string> { { "X-User-Id", userId.ToString() } };
            }

            var response = await _serviceProxy.ForwardRequestAsync("blogs", path, method, form, authToken, extraHeaders);

            var responseContent = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode,
                string.IsNullOrEmpty(responseContent) ? null : responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing multipart request for blogs/{Path}", path);
            return StatusCode(500, new { message = "Gateway error occurred" });
        }
    }

    private long ExtractUserIdFromJwt(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);
        var idClaim = jwt.Claims.FirstOrDefault(c => c.Type == "id");
        if (idClaim == null) throw new UnauthorizedAccessException("Missing id claim");
        return long.Parse(idClaim.Value);
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

    private async Task<IActionResult> ForwardFollowRequest(string serviceName, string path, HttpMethod method, long targetUserId, bool includeAuth = false)
    {
        try
        {
            _logger.LogInformation("=== FORWARD FOLLOW REQUEST START ===");
            _logger.LogInformation("Service: {ServiceName}, Path: {Path}, Method: {Method}, TargetUserId: {TargetUserId}", 
                serviceName, path, method, targetUserId);
            
            var followerId = ExtractUserIdFromJwt(ExtractTokenFromHeader());
            _logger.LogInformation("Extracted followerId: {FollowerId}", followerId);
            
            var followRequest = new { followerId = followerId, followedId = targetUserId };
            var json = System.Text.Json.JsonSerializer.Serialize(followRequest);
            _logger.LogInformation("Request body: {Json}", json);
            
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            string? authToken = null;
            if (includeAuth)
            {
                authToken = ExtractTokenFromHeader();
                _logger.LogInformation("Auth token included: {HasToken}", !string.IsNullOrEmpty(authToken));
            }

            _logger.LogInformation("Forwarding request to {ServiceName}/{Path}", serviceName, path);
            var response = await _serviceProxy.ForwardRequestAsync(serviceName, path, method, content, authToken);
            _logger.LogInformation("Received response with status: {StatusCode}", response.StatusCode);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Response content: {ResponseContent}", responseContent);
            _logger.LogInformation("=== FORWARD FOLLOW REQUEST END ===");
            
            return StatusCode((int)response.StatusCode, 
                string.IsNullOrEmpty(responseContent) ? null : responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing follow request for {ServiceName}/{Path}", serviceName, path);
            return StatusCode(500, new { message = "Gateway error occurred" });
        }
    }
}