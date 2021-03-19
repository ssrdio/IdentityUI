using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.Audit.Data
{
    public class AuditCommentEntity : IAuditBaseEntity
    {
        public long Id { get; set; }
        public string Comment { get; set; }

        public AuditEntity Audit { get; set; }
        public long AuditId { get; set; }

        public string UserId { get; set; }
        public string GroupId { get; set; }

        /// <summary>
        /// Created is in UTC timezone
        /// </summary>
        public DateTime Created { get; set; }

        public AuditCommentEntity()
        {
        }

        public AuditCommentEntity(string comment, long auditId, string userId, string groupId)
        {
            Comment = comment;
            UserId = userId;
            GroupId = groupId;
            AuditId = auditId;

            Created = DateTime.UtcNow;
        }
    }
}
