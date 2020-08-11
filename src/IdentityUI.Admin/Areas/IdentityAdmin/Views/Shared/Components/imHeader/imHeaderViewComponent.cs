using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Result;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Views.Shared.Components.imHeader
{
    public class imHeaderViewComponent : ViewComponent
    {
        public imHeaderViewComponent()
        {
        }

        public IViewComponentResult Invoke()
        {
            return View();
        }

    }
}
