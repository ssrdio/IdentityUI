using SSRD.IdentityUI.Core.Data.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Entities
{
    public class RoleAssignmentEntity : IBaseEntity
    {
        public long Id { get; private set; }

        public string RoleId { get; private set; }
        public RoleEntity Role { get; private set; }

        public string CanAssigneRoleId { get; private set; }
        public RoleEntity CanAssigneRole { get; private set; }

        public DateTimeOffset? _CreatedDate { get; set; }
        public DateTimeOffset? _ModifiedDate { get; set; }

        protected RoleAssignmentEntity()
        {
        }

        public RoleAssignmentEntity(string roleId, string canAssigneRoleId)
        {
            RoleId = roleId;
            CanAssigneRoleId = canAssigneRoleId;
        }
    }
}
