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
        public string SubjectMetadata { get; set; }

        public string GroupIdentifier { get; set; }

        public string Host { get; set; }
        public string RemoteIp { get; set; }
        public string ResourceName { get; set; }
        public string UserAgent { get; set; }
        public string TraceIdentifier { get; set; }
        public string AppVersion { get; set; }
        public string Metadata { get; set; }

        public string Created { get; set; }

        public AuditAdminDetailsModel(
            long id,
            string actionType,
            string objectType,
            string objectIdentifier,
            string objectMetadata,
            string subjectType,
            string subjectIdentifier,
            string subjectMetadata,
            string groupIdentifier,
            string host,
            string remoteIp,
            string resourceName,
            string userAgent,
            string traceIdentifier,
            string appVersion,
            string metadata,
            string created)
        {
            Id = id;
            ActionType = actionType;
            ObjectType = objectType;
            ObjectIdentifier = objectIdentifier;
            ObjectMetadata = objectMetadata;
            SubjectType = subjectType;
            SubjectIdentifier = subjectIdentifier;
            SubjectMetadata = subjectMetadata;
            GroupIdentifier = groupIdentifier;
            Host = host;
            RemoteIp = remoteIp;
            ResourceName = resourceName;
            UserAgent = userAgent;
            TraceIdentifier = traceIdentifier;
            AppVersion = appVersion;
            Metadata = metadata;
            Created = created;
        }
    }
}
