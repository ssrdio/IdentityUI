using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using SSRD.Audit.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace SSRD.Audit.Services
{
    public class HttpContextAuditDataService : IAuditSubjectDataService
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;

        protected readonly AuditOptions _auditOptions;

        public HttpContextAuditDataService(IHttpContextAccessor httpContextAccessor, IOptions<AuditOptions> auditOptions)
        {
            _httpContextAccessor = httpContextAccessor;
            _auditOptions = auditOptions.Value;
        }

        public Models.AuditSubjectData Get()
        {
            Models.AuditSubjectData auditData = new Models.AuditSubjectData(
                subjectType: Data.SubjectTypes.Human,
                subjectIdentifier: GetUserId(),
                host: GetHost(),
                remoteIp: GetRemoteIp(),
                resourceName: GetResourceName(),
                userAgent: GetUserAgent(),
                traceIdentifier: GetTraceIdentifier(),
                appVersion: GetAppVersion());

            return auditData;
        }

        public virtual string GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst(_auditOptions.UserIdClaimType)?.Value;
        }

        public virtual string GetHost()
        {
            if(_httpContextAccessor.HttpContext?.Request?.Host == null)
            {
                return null;
            }

            return _httpContextAccessor.HttpContext.Request.Host.Value;
        }

        public virtual string GetRemoteIp()
        {
            if(_auditOptions.UseXForwardedFor)
            {
                bool? exits = _httpContextAccessor.HttpContext?.Request?.Headers?.TryGetValue(_auditOptions.XForwardedForHeaderKey, out StringValues values);
                if(!exits.HasValue || !exits.Value)
                {
                    return null;
                }

                return string.Join(",", values.ToArray());
            }

            return _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        }

        public virtual string GetResourceName()
        {
            return _httpContextAccessor.HttpContext?.Request?.Path;
        }

        public virtual string GetUserAgent()
        {
            bool? exits = _httpContextAccessor.HttpContext?.Request?.Headers?.TryGetValue(HeaderNames.UserAgent, out StringValues values);
            if (!exits.HasValue || !exits.Value)
            {
                return null;
            }

            return string.Join(",", values.ToArray());
        }

        public virtual string GetTraceIdentifier()
        {
            return _httpContextAccessor.HttpContext?.TraceIdentifier;
        }

        public virtual string GetAppVersion()
        {
            if(string.IsNullOrEmpty(_auditOptions.AppVersion))
            {
                return Assembly.GetExecutingAssembly().GetName().Version?.ToString();
            }

            return _auditOptions.AppVersion;
        }
    }
}
