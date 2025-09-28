using AutoMapper;
using FluentResults;
using StakeholdersService.Common;
using StakeholdersService.Domain.RepositoryInterfaces;
using StakeholdersService.DTO;

namespace StakeholdersService.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserProfileService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public Result<UserProfileDto> GetUserProfile(long userId)
        {
            var user = _userRepository.GetProfileById(userId);
            if (user == null)
            {
                return Result.Fail(FailureCode.NotFound).WithError("User not found");
            }

            var userProfileDto = _mapper.Map<UserProfileDto>(user);
            return Result.Ok(userProfileDto);
        }

        public Result<UserProfileDto> UpdateUserProfile(long userId, UpdateUserProfileDto updateDto)
        {
            var user = _userRepository.GetProfileById(userId);
            if (user == null)
            {
                return Result.Fail(FailureCode.NotFound).WithError("User not found");
            }

            // Validate required fields if they are being updated
            if (!string.IsNullOrWhiteSpace(updateDto.Name) && string.IsNullOrWhiteSpace(updateDto.Name.Trim()))
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError("Name cannot be empty");
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Surname) && string.IsNullOrWhiteSpace(updateDto.Surname.Trim()))
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError("Surname cannot be empty");
            }

            // Apply updates only for non-null values
            if (updateDto.Name != null)
                user.Name = updateDto.Name.Trim();
            
            if (updateDto.Surname != null)
                user.Surname = updateDto.Surname.Trim();
            
            if (updateDto.ProfilePicture != null)
                user.ProfilePicture = updateDto.ProfilePicture;
            
            if (updateDto.Motto != null)
                user.Motto = updateDto.Motto;
            
            if (updateDto.Biography != null)
                user.Biography = updateDto.Biography;

            try
            {
                var updatedUser = _userRepository.UpdateProfile(user);
                var userProfileDto = _mapper.Map<UserProfileDto>(updatedUser);
                return Result.Ok(userProfileDto);
            }
            catch (Exception ex)
            {
                return Result.Fail(FailureCode.Internal).WithError("Failed to update user profile: " + ex.Message);
            }
        }
    }
}