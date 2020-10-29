using SSRD.Audit.Attributes;
using SSRD.Audit.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.Audit.Data
{
    [AuditIgnore]
    public class AuditEntity : IAuditBaseEntity
    {
        public long Id { get; set; }

        public ActionTypes ActionType { get; set; }

        public string ObjectIdentifier { get; set; }
        public string ObjectType { get; set; }
        public string ObjectMetadata { get; set; }

        public SubjectTypes SubjectType { get; set; }
        public string SubjectIdentifier { get; set; }
        public string SubjectMetadata { get; set; }

        public string GroupIdentifier { get; set; }

        public string Host { get; set; }
        public string RemoteIp { get; set; }
        public string ResourceName { get; set; }
        public string UserAgent { get; set; }
        public string TraceIdentifier { get; set; }
        public string AppVersion { get; set; }

        public string Metadata { get; set; }

        /// <summary>
        /// Created is in UTC timezone
        /// </summary>
        public DateTime Created { get; set; }

        protected AuditEntity()
        {
        }

        public AuditEntity(AuditObjectData auditObjectData, AuditSubjectData auditSubjectData)
        {
            ActionType = auditObjectData.ActionType;

            if (auditObjectData.ObjectIdentifierProperty == null)
            {
                ObjectIdentifier = auditObjectData.ObjectIdentifier;
            }
            else
            {
                ObjectIdentifier = auditObjectData.ObjectIdentifierProperty.CurrentValue.ToString();
            }

            ObjectType = auditObjectData.ObjectType;
            ObjectMetadata = auditObjectData.ObjectMetadata;

            SubjectType = auditSubjectData.SubjectType;
            SubjectIdentifier = auditSubjectData.SubjectIdentifier;
            SubjectMetadata = auditSubjectData.SubjectMetadata;

            GroupIdentifier = auditSubjectData.GroupIdentifier;

            Host = auditSubjectData.Host;
            RemoteIp = auditSubjectData.RemoteIp;
            ResourceName = auditSubjectData.ResourceName;
            UserAgent = auditSubjectData.UserAgent;
            TraceIdentifier = auditSubjectData.TraceIdentifier;
            AppVersion = auditSubjectData.AppVersion;

            Created = DateTime.UtcNow;
        }
    }
}
