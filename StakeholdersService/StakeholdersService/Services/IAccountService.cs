using FluentResults;
using Microsoft.AspNetCore.Mvc;
using StakeholdersService.Common;
using StakeholdersService.DTO;

namespace StakeholdersService.Services
{
    public interface IAccountService
    {
        Result<PagedResult<AccountDto>> GetAllAccounts(int page, int pageSize);
        Result BlockUser(long userId);
        Result UnblockUser(long userId);
    }
}