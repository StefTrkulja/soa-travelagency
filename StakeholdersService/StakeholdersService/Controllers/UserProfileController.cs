using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StakeholdersService.Controllers;
using StakeholdersService.DTO;
using StakeholdersService.Services;
using System.Security.Claims;

namespace StakeholdersService.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/user/profile")]
    public class UserProfileController : BaseApiController
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpGet]
        public ActionResult<UserProfileDto> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out long userId))
            {
                return Unauthorized("Invalid user token");
            }

            var result = _userProfileService.GetUserProfile(userId);
            return CreateResponse(result);
        }

        [HttpPatch]
        public ActionResult<UserProfileDto> UpdateProfile([FromBody] UpdateUserProfileDto updateDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out long userId))
            {
                return Unauthorized("Invalid user token");
            }

            var result = _userProfileService.UpdateUserProfile(userId, updateDto);
            return CreateResponse(result);
        }
    }
}