using Microsoft.AspNetCore.Mvc;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Attributes;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers.Group
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [GroupPermissionAuthorize(IdentityUIPermissions.IDENTITY_UI_CAN_MANAGE_GROUPS)]
    [Area(PagePath.IDENTITY_ADMIN_AREA_NAME)]
    public class GroupBaseController : Controller
    {
        protected IActionResult NotFoundView()
        {
            return View("NotFound");
        }
    }
}
