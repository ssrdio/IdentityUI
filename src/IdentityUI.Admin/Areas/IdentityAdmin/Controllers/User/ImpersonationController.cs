using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers.User
{
    [Route("/[area]/[controller]/[action]")]
    public class ImpersonationController : BaseController
    {
        private readonly IImpersonateService _impersonateService;

        public ImpersonationController(IImpersonateService impersonateService)
        {
            _impersonateService = impersonateService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Start([FromRoute] string userId)
        {
            Result result = await _impersonateService.Start(userId);

            return result.ToApiResult();
        }

        [HttpGet("/[area]/[controller]/[action]")]
        public async Task<IActionResult> Stop()
        {
            Result result = await _impersonateService.Stop();

            return result.ToApiResult();
        }
    }
}
