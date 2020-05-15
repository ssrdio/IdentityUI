using Microsoft.AspNetCore.Mvc.Rendering;
using SSRD.AdminUI.Template.Models;
using SSRD.IdentityUI.Core.Data.Enums.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Role
{
    public class NewRoleViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public RoleTypes Type { get; set; }

        public StatusAlertViewModel StatusAlert { get; set; }

        public IEnumerable<SelectListItem> RoleTypes { get; set; }

        public NewRoleViewModel(StatusAlertViewModel statusAlert, IEnumerable<SelectListItem> roleTypes)
        {
            StatusAlert = statusAlert;
            RoleTypes = roleTypes;
        }
    }
}
