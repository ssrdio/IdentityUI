using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Views.Role.Components.RoleMenu
{
    public class RoleMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string roleId, TabSelected tabSelected, string roleName)
        {
            return View("Default", new ViewModel(roleId, tabSelected, roleName));
        }

        public class ViewModel
        {
            public string RoleId { get; set; }
            public TabSelected TabSelected { get; set; }
            public string RoleName { get; set; }

            public ViewModel(string roleId, TabSelected tabSelected, string roleName)
            {
                RoleId = roleId;
                TabSelected = tabSelected;
                RoleName = roleName;
            }
        }

        public enum TabSelected
        {
            Details = 1,
            Users = 2,
        }
    }
}
