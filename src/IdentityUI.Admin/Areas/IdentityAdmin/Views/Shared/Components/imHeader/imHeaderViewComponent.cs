using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Result;
using Microsoft.AspNetCore.Http;


namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Views.Shared.Components.imHeader
{
    public class imHeaderViewComponent : ViewComponent
    {
        private readonly IManageUserService _manageUserService;
        private const string ProfileImageKey = "_ProfileImage";

        public string ProfileImage
        {
            get
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString(ProfileImageKey)))
                {
                    string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    Result<string> result = _manageUserService.GetProfileImageURL(userId);

                    if (result.Success && !string.IsNullOrEmpty(result.Value))
                    {
                        ProfileImage = result.Value;
                        return result.Value;
                    }
                }
                return HttpContext.Session.GetString(ProfileImageKey);
            }
            private set { HttpContext.Session.SetString(ProfileImageKey, value); }
        }

        public imHeaderViewComponent(IManageUserService manageUserService)
        {
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.ProfileImage = ProfileImage;

            return View();
        }

    }
}
