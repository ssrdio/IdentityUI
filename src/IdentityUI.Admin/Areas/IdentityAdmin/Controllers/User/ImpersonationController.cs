using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Models.Options;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers.User
{
    [Route("/[area]/[controller]/[action]")]
    public class ImpersonationController : BaseController
    {
        private readonly IImpersonateService _impersonateService;

        private readonly IdentityUIEndpoints _identityUIEndpoints;

        public ImpersonationController(IImpersonateService impersonateService, IOptions<IdentityUIEndpoints> identityUIEndpoints)
        {
            _impersonateService = impersonateService;
            _identityUIEndpoints = identityUIEndpoints.Value;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> Start([FromRoute] string userId)
        {
            if(!_identityUIEndpoints.AllowImpersonation)
            {
                return NotFound();
            }

            Result result = await _impersonateService.Start(userId);

            return result.ToApiResult();
        }

        [HttpGet("/[area]/[controller]/[action]")]
        public async Task<IActionResult> Stop()
        {
            if (!_identityUIEndpoints.AllowImpersonation)
            {
                return NotFound();
            }

            Result result = await _impersonateService.Stop();

            return result.ToApiResult();
        }
    }
}
