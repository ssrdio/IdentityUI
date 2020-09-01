using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Views.User.Components.UserMenu
{
    public class UserMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string userId, TabSelected tabSelected, string userName)
        {
            return View("Default", new ViewModel(userId, tabSelected, userName));
        }

        public class ViewModel
        {
            public string UserId { get; set; }
            public TabSelected TabSelected { get; set; }
            public string UserName { get; set; }

            public ViewModel(string userId, TabSelected tabSelected, string userName)
            {
                UserId = userId;
                TabSelected = tabSelected;
                UserName = userName;
            }
        }

        public enum TabSelected
        {
            Details = 1,
            Credentials = 2,
            Roles = 3,
            Sessions = 4,
            Attributes = 5,
        }
    }
}
