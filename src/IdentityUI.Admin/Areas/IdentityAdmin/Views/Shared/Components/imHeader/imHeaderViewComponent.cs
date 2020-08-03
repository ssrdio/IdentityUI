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
        private readonly IManageUserService _manageUserService;

        // TODO; TBD, Need to implement session based cache for profile image.
        //public string ProfileImage
        //{
        //    get
        //    {
        //        //if (string.IsNullOrEmpty(HttpContext.Session.GetString("_ProfileImage")))
        //        {
        //            string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //            Result<string> result = _manageUserService.GetProfileImageURL(userId);

        //            if (result.Success)
        //            {
        //                return result.Value;
        //            }
        //            return default(string);
        //        }
        //        //return HttpContext.Session.GetString("_ProfileImage");
        //    }
        //    //private set { HttpContext.Session.SetString("_ProfileImage", value);  }
        //}

        public imHeaderViewComponent(IManageUserService manageUserService)
        {
            _manageUserService = manageUserService;
        }

        public IViewComponentResult Invoke()
        {
            string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            Result<string> result = _manageUserService.GetProfileImageURL(userId);

            if (result.Success && !string.IsNullOrEmpty(result.Value))
            {
                ViewBag.ProfileImage = result.Value;
            }

            return View();
        }

    }
}
