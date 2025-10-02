using AutoMapper;
using FluentResults;
using StakeholdersService.Common;
using StakeholdersService.Domain;
using StakeholdersService.Domain.RepositoryInterfaces;
using StakeholdersService.DTO;
using System;
namespace StakeholdersService.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;

        private readonly IMapper _mapper;
    
        public AccountService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public Result<PagedResult<AccountDto>> GetAllAccounts(int page, int pageSize)
        {
            var users = _userRepository.GetPaged(page , pageSize, out int totalCount);

            var userDtos = users.Select(u => _mapper.Map<AccountDto>(u)).ToList();

            return Result.Ok(new PagedResult<AccountDto>(userDtos, totalCount));
        }

        public Result<AccountDto> BlockUser(long userId)
        {
            try
            {
                var user = _userRepository.GetById(userId);
                if (user == null)
                    return Result.Fail(FailureCode.NotFound).WithError($"User with ID {userId} not found");

                if (user.Role == UserRole.Administrator)
                    return Result.Fail(FailureCode.Forbidden).WithError("Cannot block administrator users");

                var blockedUser = _userRepository.BlockUser(userId);
                var accountDto = _mapper.Map<AccountDto>(blockedUser);
                
                return Result.Ok(accountDto);
            }
            catch (ArgumentException ex)
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError(ex.Message);
            }
        }

        public Result<AccountDto> UnblockUser(long userId)
        {
            try
            {
                var user = _userRepository.GetById(userId);
                if (user == null)
                    return Result.Fail(FailureCode.NotFound).WithError($"User with ID {userId} not found");

                var unblockedUser = _userRepository.UnblockUser(userId);
                var accountDto = _mapper.Map<AccountDto>(unblockedUser);
                
                return Result.Ok(accountDto);
            }
            catch (ArgumentException ex)
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError(ex.Message);
            }
        }
    }


}
