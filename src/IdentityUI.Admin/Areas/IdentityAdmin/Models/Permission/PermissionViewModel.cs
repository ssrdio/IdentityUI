using SSRD.AdminUI.Template.Models;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Views.Permission.Components.PermissionMenu;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Permission
{
    public class PermissionViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public StatusAlertViewModel StatusAlert { get; set; }

        public PermissionViewModel(string id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public PermissionViewModel(StatusAlertViewModel statusAlert)
        {
            StatusAlert = statusAlert;
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
