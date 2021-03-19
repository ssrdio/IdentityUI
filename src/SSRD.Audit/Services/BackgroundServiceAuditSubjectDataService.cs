using Microsoft.Extensions.Options;
using SSRD.Audit.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SSRD.Audit.Services
{
    public class BackgroundServiceAuditSubjectDataService : IAuditSubjectDataService
    {
        protected readonly IBackgroundServiceContextAccessor _backgroundServiceContextAccessor;
        private readonly AuditOptions _auditOptions;

        public BackgroundServiceAuditSubjectDataService(
            IBackgroundServiceContextAccessor backgroundServiceContextAccessor,
            IOptions<AuditOptions> auditOptions)
        {
            _backgroundServiceContextAccessor = backgroundServiceContextAccessor;
            _auditOptions = auditOptions.Value;
        }

        public virtual AuditSubjectData Get()
        {
            AuditSubjectData auditSubjectData = new AuditSubjectData(
                subjectType: Data.SubjectTypes.Machine,
                subjectIdentifier: _backgroundServiceContextAccessor.BackgroundServiceContext.Name,
                subjectMetadata: GetSubjectMetadata(),
                groupIdentifier: null,
                host: null,
                remoteIp: null,
                resourceName: null,
                userAgent: null,
                traceIdentifier: GetTraceIdentifier(),
                appVersion: _auditOptions.AppVersion);

            return auditSubjectData;
        }

        public virtual string GetSubjectMetadata()
        {
            string userId = _backgroundServiceContextAccessor.BackgroundServiceContext.Claims[_auditOptions.UserIdClaimType];
            string groupId = _backgroundServiceContextAccessor.BackgroundServiceContext.Claims[_auditOptions.GroupIdClaimType];

            BackgroundServiceSubjectMetaDataModel backgroundServiceSubjectMetaDataModel = new BackgroundServiceSubjectMetaDataModel(
                userId: userId,
                groupId: groupId);

            string json = JsonSerializer.Serialize(backgroundServiceSubjectMetaDataModel);

            return json;
        }

        public virtual string GetTraceIdentifier()
        {
            return _backgroundServiceContextAccessor.BackgroundServiceContext.Identifier;
        }
    }
}
