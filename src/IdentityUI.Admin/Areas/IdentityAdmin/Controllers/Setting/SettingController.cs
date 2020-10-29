using Microsoft.AspNetCore.Mvc;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers.Setting
{
    public class SettingController : BaseController
    {
        public IActionResult Index()
        {
            return LocalRedirect(PagePath.SETTING_EMAIL);
        }
    }
}
