
using FluentResults;
using Microsoft.IdentityModel.Tokens;
using StakeholdersService.Domain;
using StakeholdersService.DTO;
using StakeholdersService.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StakeholdersService.Authentication;

public class JwtGenerator : ITokenGenerator
{
    private readonly string _key = Environment.GetEnvironmentVariable("JWT_KEY") ?? "explorer_secret_key";
    private readonly string _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "explorer";
    private readonly string _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "explorer-front.com";

    public Result<AuthenticationTokensDto> GenerateAccessToken(User user)
    {
        var authenticationResponse = new AuthenticationTokensDto();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("id", user.Id.ToString()),
            new("username", user.Username),
            new(ClaimTypes.Role, user.GetPrimaryRoleName())
        };

        var jwt = CreateToken(claims, 60 * 24);
        authenticationResponse.Id = user.Id;
        authenticationResponse.AccessToken = jwt;

        return authenticationResponse;
    }

    public Result<AuthenticatedTokenDto> DecomposeAccessToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

        if (jsonToken == null)
        {
            return Result.Fail("Invalid token");
        }

        var id = jsonToken.Claims.First(claim => claim.Type == "id").Value;
        var username = jsonToken.Claims.First(claim => claim.Type == "username").Value;
        var role = jsonToken.Claims.First(claim => claim.Type == ClaimTypes.Role).Value;

        return new AuthenticatedTokenDto
        {
            UserId = long.Parse(id),
            Username = username,
            Role = role
        };
    }

    private string CreateToken(IEnumerable<Claim> claims, double expirationTimeInMinutes)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _issuer,
            _audience,
            claims,
            expires: DateTime.Now.AddMinutes(expirationTimeInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}