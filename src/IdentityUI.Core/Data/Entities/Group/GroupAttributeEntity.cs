using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Entities.Group
{
    public class GroupAttributeEntity : IBaseEntity
    {
        public long Id { get; private set; }

        public string Key { get; private set; }
        public string Value { get; private set; }

        public string GroupId { get; private set; }
        public GroupEntity Group { get; private set; }

        public DateTimeOffset? _CreatedDate { get; set; }
        public DateTimeOffset? _ModifiedDate { get; set; }

        protected GroupAttributeEntity()
        {
        }

        public GroupAttributeEntity(string key, string value, string groupId)
        {
            Key = key;
            Value = value;
            GroupId = groupId;
        }

        public void Update(string value)
        {
            Value = value;
        }
    }
}
