using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Permission
{
    public class PermissionTableModel
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public PermissionTableModel(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
