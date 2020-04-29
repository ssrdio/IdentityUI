using Microsoft.AspNetCore.Mvc.Rendering;
using SSRD.AdminUI.Template.Models;
using SSRD.IdentityUI.Core.Data.Enums.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Role
{
    public class RoleDetailViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public RoleTypes Type { get; set; }

        public StatusAlertViewModel StatusAlert { get; set; }
        public IEnumerable<SelectListItem> RoleTypes { get; set; }

        public RoleDetailViewModel(StatusAlertViewModel statusAlert, IEnumerable<SelectListItem> roleTypes)
        {
            StatusAlert = statusAlert;
            RoleTypes = roleTypes;
        }

        public RoleDetailViewModel(string id, string name, string description, RoleTypes type)
        {
            Id = id;
            Name = name;
            Description = description;
            Type = type;
        }
    }
}
