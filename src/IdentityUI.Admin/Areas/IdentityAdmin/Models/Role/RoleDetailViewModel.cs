using SSRD.AdminUI.Template.Models;
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

        public StatusAlertViewModel StatusAlert { get; set; }

        public RoleDetailViewModel(StatusAlertViewModel statusAlert)
        {
            StatusAlert = statusAlert;
        }

        public RoleDetailViewModel(string id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}
