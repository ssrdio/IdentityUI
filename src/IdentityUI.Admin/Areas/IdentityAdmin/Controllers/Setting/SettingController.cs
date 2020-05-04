using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
