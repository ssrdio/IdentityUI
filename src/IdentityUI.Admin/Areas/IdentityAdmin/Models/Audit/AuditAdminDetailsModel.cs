using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Audit
{
    public class AuditAdminDetailsModel
    {
        public long Id { get; set; }

        public string ActionType { get; set; }
        public string ObjectType { get; set; }
        public string ObjectIdentifier { get; set; }
        public string ObjectMetadata { get; set; }

        public string SubjectType { get; set; }
        public string SubjectIdentifier { get; set; }

        public string Host { get; set; }
        public string RemoteIp { get; set; }
        public string ResourceName { get; set; }
        public string UserAgent { get; set; }
        public string TraceIdentifier { get; set; }
        public string AppVersion { get; set; }

        public string Created { get; set; }

        public AuditAdminDetailsModel(
            long id,
            string actionType,
            string objectType,
            string objectIdentifier,
            string objectMetadata,
            string subjectType,
            string subjectIdentifier,
            string host,
            string remoteIp,
            string resourceName,
            string userAgent,
            string traceIdentifier,
            string appVersion,
            string created)
        {
            Id = id;
            ActionType = actionType;
            ObjectType = objectType;
            ObjectIdentifier = objectIdentifier;
            ObjectMetadata = objectMetadata;
            SubjectType = subjectType;
            SubjectIdentifier = subjectIdentifier;
            Host = host;
            RemoteIp = remoteIp;
            ResourceName = resourceName;
            UserAgent = userAgent;
            TraceIdentifier = traceIdentifier;
            AppVersion = appVersion;
            Created = created;
        }
    }
}
