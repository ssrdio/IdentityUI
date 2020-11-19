using System;
using System.Collections.Generic;

namespace SSRD.IdentityUI.Core.Data.Entities.Group
{
    public class GroupEntity : IBaseEntity, ISoftDelete
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public ICollection<GroupUserEntity> Users { get; set; }
        public ICollection<GroupAttributeEntity> GroupAttributes { get; set; }

        public ICollection<InviteEntity> Invites { get; set; }

        public DateTimeOffset? _CreatedDate { get; set; }
        public DateTimeOffset? _ModifiedDate { get; set; }
        public DateTimeOffset? _DeletedDate { get; set; }

        protected GroupEntity()
        {

        }

        public GroupEntity(string name)
        {
            Name = name;
        }
    }
}
