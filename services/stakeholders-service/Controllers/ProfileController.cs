using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StakeholdersService.DTO;
using StakeholdersService.Services;
using System.Security.Claims;

namespace StakeholdersService.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ProfileController : BaseApiController
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public ActionResult<UserProfileDto> GetMyProfile()
        {
            var userId = GetCurrentUserId();
            var result = _profileService.GetUserProfile(userId);
            return CreateResponse(result);
        }

        [HttpGet("{userId}")]
        public ActionResult<UserProfileDto> GetUserProfile(long userId)
        {
            var result = _profileService.GetUserProfile(userId);
            return CreateResponse(result);
        }

        [HttpPut]
        public ActionResult<UserProfileDto> UpdateMyProfile([FromBody] UpdateProfileDto updateProfileDto)
        {
            var userId = GetCurrentUserId();
            var result = _profileService.UpdateUserProfile(userId, updateProfileDto);
            return CreateResponse(result);
        }

        [HttpPut("password")]
        public ActionResult<UserProfileDto> UpdateMyPassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var userId = GetCurrentUserId();
            var result = _profileService.UpdatePassword(userId, changePasswordDto);
            return CreateResponse(result);
        }

        [HttpGet("all")]
        public ActionResult<List<UserProfileDto>> GetAllUsers()
        {
            var result = _profileService.GetAllUsers();
            return CreateResponse(result);
        }

        // Location endpoints
        [HttpPost("location")]
        public ActionResult<UserLocationResponseDto> UpdateMyLocation([FromBody] UserLocationDto locationDto)
        {
            var userId = GetCurrentUserId();
            var result = _profileService.UpdateUserLocation(userId, locationDto);
            return CreateResponse(result);
        }

        [HttpGet("location")]
        public ActionResult<UserLocationResponseDto> GetMyLocation()
        {
            var userId = GetCurrentUserId();
            var result = _profileService.GetUserLocation(userId);
            return CreateResponse(result);
        }

        [HttpGet("location/{userId}")]
        public ActionResult<UserLocationResponseDto> GetUserLocation(long userId)
        {
            var result = _profileService.GetUserLocation(userId);
            return CreateResponse(result);
        }

        [HttpDelete("location")]
        public ActionResult<UserLocationResponseDto> ClearMyLocation()
        {
            var userId = GetCurrentUserId();
            var result = _profileService.ClearUserLocation(userId);
            return CreateResponse(result);
        }

        [HttpGet("nearby")]
        public ActionResult<List<UserLocationResponseDto>> GetUsersNearby(
            [FromQuery] decimal latitude, 
            [FromQuery] decimal longitude, 
            [FromQuery] double radiusKm = 10.0)
        {
            var result = _profileService.GetUsersNearLocation(latitude, longitude, radiusKm);
            return CreateResponse(result);
        }

        private long GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("id");
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID in token");
            }
            return userId;
        }
    }
}