using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Data.Entities.Identity
{
    public class UserRoleEntity : IdentityUserRole<string>, IBaseEntity
    {
        public DateTimeOffset? _CreatedDate { get; set; }
        public DateTimeOffset? _ModifiedDate { get; set; }

        public virtual AppUserEntity User { get; set; }
        public virtual RoleEntity Role { get; set; }

        public UserRoleEntity()
        {
        }

        public UserRoleEntity(string userId, string roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }
    }
}
