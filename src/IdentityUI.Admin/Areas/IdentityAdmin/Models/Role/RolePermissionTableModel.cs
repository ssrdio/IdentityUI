using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Role
{
    public class RolePermissionTableModel
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public RolePermissionTableModel(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
