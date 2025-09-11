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

       




    }


}
