
using FluentResults;
using StakeholdersService.Domain;
using StakeholdersService.DTO;

namespace StakeholdersService.Services;

public interface ITokenGenerator
{
    Result<AuthenticationTokensDto> GenerateAccessToken(User user);
    Result<AuthenticatedTokenDto> DecomposeAccessToken(string token);
}