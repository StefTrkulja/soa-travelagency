using FluentResults;
using StakeholdersService.Domain;
using StakeholdersService.DTO;

namespace StakeholdersService.UseCases
{
    public interface ITokenGenerator
    {
        Result<AuthenticationTokensDto> GenerateAccessToken(User user, long userId);

    }
}
