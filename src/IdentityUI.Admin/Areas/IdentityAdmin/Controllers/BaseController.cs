using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models;
using System.Diagnostics;
using SSRD.IdentityUI.Core.Data.Models.Constants;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize(Roles = IdentityUIRoles.IDENTITY_MANAGMENT_ROLE)]
    [Area(PagePath.IDENTITY_ADMIN_AREA_NAME)]
    public class BaseController : Controller
    {
        public BaseController()
        {

        }

        [DebuggerStepThrough]
        protected string GetUserId()
        {
            //return (User.Identity as ClaimsIdentity)?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        protected IActionResult NotFoundView()
        {
            return View("NotFound");
        }
    }
}
