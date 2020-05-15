using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Views.Role.Components.RoleMenu;
using SSRD.IdentityUI.Core.Data.Enums.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using static SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Views.Role.Components.RoleMenu.RoleMenuViewComponent;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Role
{
    public class RoleMenuViewModel
    {
        public string RoleId { get; set; }
        public string Name { get; set; }
        public RoleTypes Type { get; set; }

        public RoleMenuViewModel(string roleId, string name, RoleTypes type)
        {
            RoleId = roleId;
            Name = name;
            Type = type;
        }

        public RoleMenuViewComponent.ViewModel ToComponentViewModel(TabSelected tabSelected)
        {
            return new ViewModel(RoleId, Name, Type, tabSelected);
        }
    }
}
