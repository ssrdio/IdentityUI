using SSRD.IdentityUI.Core.Data.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Entities.Group
{
    public class GroupEntity : IBaseEntity
    {
        public string Id { get; private set; }
        public string Name { get; private set; }

        public ICollection<GroupUserEntity> Users { get; private set; }
        public ICollection<GroupAttributeEntity> GroupAttributes { get; set; }

        public DateTimeOffset? _CreatedDate { get; set; }
        public DateTimeOffset? _ModifiedDate { get; set; }

        protected GroupEntity()
        {

        }

        public GroupEntity(string name)
        {
            Name = name;
        }
    }
}
