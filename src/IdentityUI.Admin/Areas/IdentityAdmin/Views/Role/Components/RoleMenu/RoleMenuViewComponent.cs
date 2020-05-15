using Microsoft.AspNetCore.Mvc;
using SSRD.IdentityUI.Core.Data.Enums.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Views.Role.Components.RoleMenu
{
    public class RoleMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ViewModel viewModel)
        {
            return View("Default", viewModel);
        }

        public class ViewModel
        {
            public string RoleId { get; set; }
            public string RoleName { get; set; }
            public RoleTypes RoleType { get; set; }

            public TabSelected TabSelected { get; set; }


            public ViewModel(string roleId, string roleName, RoleTypes roleType, TabSelected tabSelected)
            {
                RoleId = roleId;
                RoleName = roleName;
                RoleType = roleType;

                TabSelected = tabSelected;
            }
        }

        public enum TabSelected
        {
            Details = 1,
            Users = 2,
            Assignments = 3,
            Permissions = 4,
        }
    }
}
