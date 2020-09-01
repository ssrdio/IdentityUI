using SSRD.IdentityUI.Core.Data.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Entities.User
{
    public class UserAttributeEntity : IBaseEntity
    {
        public long Id { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }

        public string UserId { get; set; }
        public AppUserEntity User { get; set; }
        public DateTimeOffset? _CreatedDate { get; set; }
        public DateTimeOffset? _ModifiedDate { get; set; }

        public UserAttributeEntity(string key, string value, string userId)
        {
            Key = key;
            Value = value;
            UserId = userId;
        }

        public UserAttributeEntity(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
