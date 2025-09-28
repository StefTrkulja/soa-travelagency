using FluentResults;
using StakeholdersService.DTO;

namespace StakeholdersService.Services
{
    public interface IUserProfileService
    {
        Result<UserProfileDto> GetUserProfile(long userId);
        Result<UserProfileDto> UpdateUserProfile(long userId, UpdateUserProfileDto updateDto);
    }
}