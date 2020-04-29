using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Role
{
    public class RoleAssignmentTableModel
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public RoleAssignmentTableModel(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
