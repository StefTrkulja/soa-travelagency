
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StakeholdersService;
using StakeholdersService.Common;
using StakeholdersService.DTO;
using StakeholdersService.Services;

namespace StakeholdersService.Controllers
{


    [ApiController]
    [Authorize(Policy = "administratorPolicy")]
    [Route("api/administrator/account")]
    public class AccountController : BaseApiController
    {

        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpGet]
        public ActionResult<PagedResult<AccountDto>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = _accountService.GetAllAccounts(page, pageSize);
            return CreateResponse(result);
        }




    }
}
