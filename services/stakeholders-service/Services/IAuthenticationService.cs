using FluentResults;
using StakeholdersService.DTO;

namespace StakeholdersService.Services
{
    public interface IAuthenticationService
    {
        Result<AuthenticationTokensDto> RegisterUser(AccountRegistrationDto account);
        Result<AuthenticationTokensDto> Login(CredentialsDto credentials);

    }
}
