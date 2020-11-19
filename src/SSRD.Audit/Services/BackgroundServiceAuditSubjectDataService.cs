using Microsoft.Extensions.Options;
using SSRD.Audit.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.Audit.Services
{
    public class BackgroundServiceAuditSubjectDataService : IAuditSubjectDataService
    {
        protected readonly IBackgroundServiceContextAccessor _backgroundServiceContextAccessor;

        public BackgroundServiceAuditSubjectDataService(IBackgroundServiceContextAccessor backgroundServiceContextAccessor)
        {
            _backgroundServiceContextAccessor = backgroundServiceContextAccessor;
        }

        public virtual AuditSubjectData Get()
        {
            AuditSubjectData auditSubjectData = new AuditSubjectData(
                subjectType: Data.SubjectTypes.Machine,
                subjectIdentifier: _backgroundServiceContextAccessor.BackgroundServiceContext.Name,
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
