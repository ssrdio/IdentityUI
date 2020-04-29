using Microsoft.AspNetCore.Identity;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Enums.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Data.Entities.Identity
{
    public class RoleEntity : IdentityRole, IBaseEntity
    {
        public DateTimeOffset? _CreatedDate { get; set; }
        public DateTimeOffset? _ModifiedDate { get; set; }

        public string Description { get; set; }
        public RoleTypes Type { get; set; }

        public virtual ICollection<UserRoleEntity> UserRoles { get; set; }
        public virtual ICollection<RoleClaimEntity> RoleClaims { get; set; }

        public virtual ICollection<RoleAssignmentEntity> CanAssigne { get; set; }
        public virtual ICollection<RoleAssignmentEntity> CanBeAssignedBy { get; set; }

        public virtual ICollection<PermissionRoleEntity> Permissions { get; set; }

        protected RoleEntity()
        {
        }

        public RoleEntity(string name, string description, RoleTypes type)
        {
            Name = name;
            Description = description;
            Type = type;
        }
    }
}
