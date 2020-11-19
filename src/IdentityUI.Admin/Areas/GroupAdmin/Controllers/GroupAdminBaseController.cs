using Microsoft.AspNetCore.Mvc;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Attributes;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Controllers
{
    [GroupAdminAuthorize]
    [Area(GroupAdminPaths.GROUP_ADMIN_AREA_NAME)]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/[area]/[controller]")]
    public class GroupAdminBaseController : Controller
    {
        protected IActionResult NotFoundView()
        {
            return View("NotFound");
        }
    }
}
