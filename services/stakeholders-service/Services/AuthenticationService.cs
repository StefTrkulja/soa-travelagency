using Microsoft.AspNetCore.Authentication;
using StakeholdersService.Domain.RepositoryInterfaces;
using StakeholdersService.Services;
using FluentResults;
using StakeholdersService.DTO;
using StakeholdersService.Common;
using StakeholdersService.Domain;
using System;

namespace StakeholdersService.Services
{
    public class AuthenticationService: IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenGenerator _tokenGenerator;
    
        public AuthenticationService(IUserRepository userRepository, ITokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
        }
        
        public Result<AuthenticationTokensDto> Login(CredentialsDto credentials)
        {
            var user = _userRepository.GetActiveByName(credentials.Username);
            if (user == null || credentials.Password != user.Password) return Result.Fail(FailureCode.NotFound);

            if (user.IsBlocked())
                return Result.Fail(FailureCode.Forbidden).WithError("User account is blocked");

            return _tokenGenerator.GenerateAccessToken(user);
        }

        public Result<AuthenticationTokensDto> RegisterUser(AccountRegistrationDto account)
        {
            if (_userRepository.Exists(account.Username)) return Result.Fail(FailureCode.NonUniqueUsername);

            try
            {
                var user = _userRepository.Create(new User(account.Username, account.Password, account.Email, account.UserRole));
             

                return _tokenGenerator.GenerateAccessToken(user);
            }
            catch (ArgumentException e)
            {
                return Result.Fail(FailureCode.InvalidArgument).WithError(e.Message);
            }
        }


    }

}
