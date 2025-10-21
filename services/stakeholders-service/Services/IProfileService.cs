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
        
        // Location methods
        Result<UserLocationResponseDto> UpdateUserLocation(long userId, UserLocationDto locationDto);
        Result<UserLocationResponseDto> GetUserLocation(long userId);
        Result<UserLocationResponseDto> ClearUserLocation(long userId);
        Result<List<UserLocationResponseDto>> GetUsersNearLocation(decimal latitude, decimal longitude, double radiusKm);
    }
}