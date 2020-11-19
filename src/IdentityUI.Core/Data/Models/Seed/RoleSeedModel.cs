using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Enums.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Models
{
    public class RoleSeedModel
    {
        public string Name { get; set; }
        public RoleTypes Type { get; set; }
        public string Description { get; set; }

        public List<string> Permissions { get; set; }
        public List<string> RoleAssignments { get; set; }

        public RoleSeedModel(string name, RoleTypes type, string description, List<string> permissions)
        {
            Name = name;
            Type = type;
            Description = description;
            Permissions = permissions;
            RoleAssignments = new List<string>();
        }

        public RoleSeedModel(string name, RoleTypes type, string description, List<string> permissions, List<string> roleAssignments)
        {
            Name = name;
            Type = type;
            Description = description;
            Permissions = permissions;
            RoleAssignments = roleAssignments;
        }

        internal RoleEntity ToEntity()
        {
            return new RoleEntity(
                name: Name,
                description: Description,
                type: Type);
        }
    }
}
