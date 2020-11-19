using Microsoft.Extensions.Options;
using SSRD.Audit.Models;

namespace SSRD.Audit.Services
{
    public class DefaultAuditSubjectService : IAuditSubjectDataService
    {
        protected readonly AuditOptions _auditOptions;

        public DefaultAuditSubjectService(IOptions<AuditOptions> auditOptions)
        {
            _auditOptions = auditOptions.Value;
        }

        public virtual AuditSubjectData Get()
        {
            AuditSubjectData auditSubjectData = new AuditSubjectData(
                subjectType: Data.SubjectTypes.Machine,
                subjectIdentifier: _auditOptions.DefaultSubjectName,
                subjectMetadata: null,
                groupIdentifier: null,
                host: null,
                remoteIp: null,
                resourceName: null,
                userAgent: null,
                traceIdentifier: null,
                appVersion: null);

            return auditSubjectData;
        }
    }
}
