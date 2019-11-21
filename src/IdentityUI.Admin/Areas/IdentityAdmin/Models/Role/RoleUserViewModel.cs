using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Role
{
    public class RoleUserViewModel
    {
        [DisplayName("Role id")]
        public string RoleId { get; set; }
        public string Name { get; set; }

        public RoleUserViewModel(string roleId, string name)
        {
            RoleId = roleId;
            Name = name;
        }
    }
}
