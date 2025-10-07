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

        [HttpGet("all")]
        public ActionResult<List<UserProfileDto>> GetAllUsers()
        {
            var result = _profileService.GetAllUsers();
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