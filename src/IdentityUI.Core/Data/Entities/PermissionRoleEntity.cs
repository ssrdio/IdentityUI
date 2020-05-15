using SSRD.IdentityUI.Core.Data.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Entities
{
    public class PermissionRoleEntity : IBaseEntity
    {
        public long Id { get; private set; }

        public string PermissionId { get; private set; }
        public PermissionEntity Permission { get; private set; }

        public string RoleId { get; private set; }
        public RoleEntity Role { get; private set; }

        public DateTimeOffset? _CreatedDate { get; set; }
        public DateTimeOffset? _ModifiedDate { get; set; }

        protected PermissionRoleEntity()
        {
        }

        public PermissionRoleEntity(string permissionId, string roleId)
        {
            PermissionId = permissionId;
            RoleId = roleId;
        }
    }
}
