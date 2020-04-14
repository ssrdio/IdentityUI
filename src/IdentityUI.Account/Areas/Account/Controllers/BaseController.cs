using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SSRD.IdentityUI.Account.Areas.Account.Models;
using SSRD.IdentityUI.Core.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SSRD.IdentityUI.Account.Areas.Account.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    [Area(PagePath.ACCOUNT_AREA_NAME)]
    public class BaseController : Controller
    {
        [DebuggerStepThrough]
        protected string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [DebuggerStepThrough]
        protected string GetSessionCode()
        {
            return User.FindFirstValue(IdentityManagementClaims.SESSION_CODE);
        }

        [DebuggerStepThrough]
        protected string GetIp()
        {
            return Request.HttpContext.Connection.RemoteIpAddress.ToString();
        }
    }
}
