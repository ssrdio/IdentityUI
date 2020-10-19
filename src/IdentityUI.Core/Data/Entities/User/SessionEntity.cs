using SSRD.Audit.Attributes;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Enums.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Entities
{
    [AuditIgnore]
    public class SessionEntity : IBaseEntity, ISoftDelete
    {
        public DateTimeOffset? _CreatedDate { get; set; }
        public DateTimeOffset? _ModifiedDate { get; set; }
        public DateTimeOffset? _DeletedDate { get; set; }

        public long Id { get; set; }
        public string Ip { get; set; }
        public string Code { get; set; }

        public DateTimeOffset LastAccess { get; set; }
        public SessionEndTypes? EndType { get; set; }

        public string UserId { get; set; }
        public AppUserEntity User { get; set; }

        public SessionEntity()
        {

        }

        public SessionEntity(string ip, string userId, string code)
        {
            Ip = ip;
            UserId = userId;
            Code = code;

            LastAccess = DateTimeOffset.UtcNow;
        }
    }
}
