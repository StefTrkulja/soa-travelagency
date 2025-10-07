using FluentResults;
using StakeholdersService.DTO;

namespace StakeholdersService.Services
{
    public interface IProfileService
    {
        Result<UserProfileDto> GetUserProfile(long userId);
        Result<UserProfileDto> UpdateUserProfile(long userId, UpdateProfileDto updateProfileDto);
    }
}