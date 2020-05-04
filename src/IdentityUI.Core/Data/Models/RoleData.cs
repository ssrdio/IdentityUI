using SSRD.IdentityUI.Core.Data.Enums.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Models
{
    public class RoleData
    {
        public string Name { get; set; }
        public RoleTypes Type { get; set; }
        public string Description { get; set; }

        public List<PermissionData> Permissions { get; set; }

        public RoleData(string name, RoleTypes type, string description, List<PermissionData> permissions)
        {
            Name = name;
            Type = type;
            Description = description;
            Permissions = permissions;
        }
    }
}
