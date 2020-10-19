using SSRD.Audit.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Entities
{
    public class PermissionEntity : IBaseEntity
    {
        public string Id { get; private set; }

        public string Name { get; private set; }
        public string Description { get; private set; }

        public ICollection<PermissionRoleEntity> Roles { get; private set; }

        public DateTimeOffset? _CreatedDate { get; set; }
        public DateTimeOffset? _ModifiedDate { get; set; }

        protected PermissionEntity()
        {
        }

        public PermissionEntity(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public void Update(string description)
        {
            Description = description;
        }
    }
}
