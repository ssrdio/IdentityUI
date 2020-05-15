using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Views.Permission.Components.PermissionMenu;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Permission
{
    public class PermissionMenuViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public PermissionMenuViewModel(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public PermissionMenuViewComponent.ViewModel ToMenuViewComponent(PermissionMenuViewComponent.TabSelected tabSelected)
        {
            return new PermissionMenuViewComponent.ViewModel(
                id: Id,
                name: Name,
                tabSelected: tabSelected);
        }
    }
}
