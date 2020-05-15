using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models;
using SSRD.IdentityUI.Core.Data.Models.Constants;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers.Setting
{
    [Authorize(Roles = IdentityUIRoles.IDENTITY_MANAGMENT_ROLE)]
    public class SettingController : BaseController
    {
        public IActionResult Index()
        {
            return LocalRedirect(PagePath.SETTING_EMAIL);
        }
    }
}
