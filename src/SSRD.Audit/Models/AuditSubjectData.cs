using SSRD.Audit.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.Audit.Models
{
    public class AuditSubjectData
    {
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

        public AuditSubjectData(
            SubjectTypes subjectType,
            string subjectIdentifier,
            string subjectMetadata,
            string groupIdentifier,
            string host,
            string remoteIp,
            string resourceName,
            string userAgent,
            string traceIdentifier,
            string appVersion)
        {
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
        }
    }
}
