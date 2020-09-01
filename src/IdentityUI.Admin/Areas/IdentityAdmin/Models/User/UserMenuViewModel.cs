using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Views.User.Components.UserMenu;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.User
{
    public class UserMenuViewModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }

        public UserMenuViewModel(string userId, string username)
        {
            UserId = userId;
            Username = username;
        }

        public UserMenuViewComponent.ViewModel ToViewComponentModel(UserMenuViewComponent.TabSelected tabSelected)
        {
            return new UserMenuViewComponent.ViewModel(
                userId: UserId,
                tabSelected: tabSelected,
                userName: Username);
        }
    }
}
