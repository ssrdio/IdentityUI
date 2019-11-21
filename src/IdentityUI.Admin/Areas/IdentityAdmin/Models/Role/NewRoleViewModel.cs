using SSRD.AdminUI.Template.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Role
{
    public class NewRoleViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public StatusAlertViewModel StatusAlert { get; set; }

        public NewRoleViewModel(StatusAlertViewModel statusAlert)
        {
            StatusAlert = statusAlert;
        }
    }
}
