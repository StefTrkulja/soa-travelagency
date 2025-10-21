using AutoMapper;
using FluentResults;
using StakeholdersService.Common;
using StakeholdersService.Domain.RepositoryInterfaces;
using StakeholdersService.DTO;

namespace StakeholdersService.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public ProfileService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public Result<UserProfileDto> GetUserProfile(long userId)
        {
            try
            {
                var user = _userRepository.GetUserProfile(userId);
                if (user == null)
                    return Result.Fail(FailureCode.NotFound).WithError($"User with ID {userId} not found");

                var profileDto = _mapper.Map<UserProfileDto>(user);
                return Result.Ok(profileDto);
            }
            catch (Exception ex)
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError(ex.Message);
            }
        }

        public Result<UserProfileDto> UpdateUserProfile(long userId, UpdateProfileDto updateProfileDto)
        {
            try
            {
                var updatedUser = _userRepository.UpdateUserProfile(
                    userId,
                    updateProfileDto.Email,
                    updateProfileDto.Name,
                    updateProfileDto.Surname,
                    updateProfileDto.ProfilePicture,
                    updateProfileDto.Biography,
                    updateProfileDto.Motto
                );

                var profileDto = _mapper.Map<UserProfileDto>(updatedUser);
                return Result.Ok(profileDto);
            }
            catch (ArgumentException ex)
            {
                return Result.Fail(FailureCode.NotFound).WithError(ex.Message);
            }
            catch (Exception ex)
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError(ex.Message);
            }
        }

        public Result<UserProfileDto> UpdatePassword(long userId, ChangePasswordDto changePasswordDto)
        {
            try
            {
                var updatedUser = _userRepository.UpdatePassword(
                    userId,
                    changePasswordDto.CurrentPassword,
                    changePasswordDto.NewPassword
                );

                var profileDto = _mapper.Map<UserProfileDto>(updatedUser);
                return Result.Ok(profileDto);
            }
            catch (ArgumentException ex)
            {
                // Check if it's a password validation error or user not found error
                if (ex.Message.Contains("password", StringComparison.OrdinalIgnoreCase))
                {
                    return Result.Fail(FailureCode.InvalidArgument).WithError(ex.Message);
                }
                return Result.Fail(FailureCode.NotFound).WithError(ex.Message);
            }
            catch (Exception ex)
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError(ex.Message);
            }
        }

        public Result<List<UserProfileDto>> GetAllUsers()
        {
            try
            {
                var users = _userRepository.GetAllUsers();
                var userDtos = _mapper.Map<List<UserProfileDto>>(users);
                return Result.Ok(userDtos);
            }
            catch (Exception ex)
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError(ex.Message);
            }
        }

        public Result<UserLocationResponseDto> UpdateUserLocation(long userId, UserLocationDto locationDto)
        {
            try
            {
                var updatedUser = _userRepository.UpdateUserLocation(userId, locationDto.Latitude, locationDto.Longitude);
                var locationResponse = new UserLocationResponseDto
                {
                    UserId = updatedUser.Id,
                    Username = updatedUser.Username,
                    Latitude = updatedUser.Latitude,
                    Longitude = updatedUser.Longitude,
                    LocationUpdatedAt = updatedUser.LocationUpdatedAt,
                    HasLocation = updatedUser.HasLocation()
                };
                return Result.Ok(locationResponse);
            }
            catch (ArgumentException ex)
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError(ex.Message);
            }
            catch (Exception ex)
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError(ex.Message);
            }
        }

        public Result<UserLocationResponseDto> GetUserLocation(long userId)
        {
            try
            {
                var user = _userRepository.GetUserLocation(userId);
                if (user == null)
                {
                    return Result.Fail(FailureCode.NotFound).WithError($"User with ID {userId} not found");
                }

                var locationResponse = new UserLocationResponseDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Latitude = user.Latitude,
                    Longitude = user.Longitude,
                    LocationUpdatedAt = user.LocationUpdatedAt,
                    HasLocation = user.HasLocation()
                };
                return Result.Ok(locationResponse);
            }
            catch (Exception ex)
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError(ex.Message);
            }
        }

        public Result<UserLocationResponseDto> ClearUserLocation(long userId)
        {
            try
            {
                var updatedUser = _userRepository.ClearUserLocation(userId);
                var locationResponse = new UserLocationResponseDto
                {
                    UserId = updatedUser.Id,
                    Username = updatedUser.Username,
                    Latitude = updatedUser.Latitude,
                    Longitude = updatedUser.Longitude,
                    LocationUpdatedAt = updatedUser.LocationUpdatedAt,
                    HasLocation = updatedUser.HasLocation()
                };
                return Result.Ok(locationResponse);
            }
            catch (ArgumentException ex)
            {
                return Result.Fail(FailureCode.NotFound).WithError(ex.Message);
            }
            catch (Exception ex)
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError(ex.Message);
            }
        }

        public Result<List<UserLocationResponseDto>> GetUsersNearLocation(decimal latitude, decimal longitude, double radiusKm)
        {
            try
            {
                var users = _userRepository.GetUsersWithLocationInRadius(latitude, longitude, radiusKm);
                var locationResponses = users.Select(user => new UserLocationResponseDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Latitude = user.Latitude,
                    Longitude = user.Longitude,
                    LocationUpdatedAt = user.LocationUpdatedAt,
                    HasLocation = user.HasLocation()
                }).ToList();

                return Result.Ok(locationResponses);
            }
            catch (Exception ex)
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError(ex.Message);
            }
        }
    }
}