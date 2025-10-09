using FluentResults;
using StakeholdersService.DTO;

namespace StakeholdersService.Services
{
    public interface IProfileService
    {
        Result<UserProfileDto> GetUserProfile(long userId);
        Result<UserProfileDto> UpdateUserProfile(long userId, UpdateProfileDto updateProfileDto);
        Result<UserProfileDto> UpdatePassword(long userId, ChangePasswordDto changePasswordDto);
        Result<List<UserProfileDto>> GetAllUsers();
    }
}