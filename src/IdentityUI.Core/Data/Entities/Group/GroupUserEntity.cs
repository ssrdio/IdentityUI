using SSRD.IdentityUI.Core.Data.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Entities.Group
{
    public class GroupUserEntity : IBaseEntity
    {
        public long Id { get; private set; }

        public string UserId { get; private set; }
        public AppUserEntity User { get; set; }

        public string GroupId { get; private set; }
        public GroupEntity Group { get; private set; }

        public string RoleId { get; private set; }
        public RoleEntity Role { get; private set; }

        public DateTimeOffset? _CreatedDate { get; set; }
        public DateTimeOffset? _ModifiedDate { get; set; }

        protected GroupUserEntity()
        {

        }

        public GroupUserEntity(string userId, string groupId, string roleId)
        {
            UserId = userId;
            GroupId = groupId;
            RoleId = roleId;
        }

        public void UpdateRole(string roleId)
        {
            RoleId = roleId;
        }
    }
}
