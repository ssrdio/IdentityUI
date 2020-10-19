using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace SSRD.Audit.Models
{
    public class AuditOptions
    {
        /// <summary>
        /// Claim type name where user identifier is stored
        /// </summary>
        public string UserIdClaimType { get; set; } = ClaimTypes.NameIdentifier;

        /// <summary>
        /// If the value for RemoteIp should be set from the header specified in <see cref="XForwardedForHeaderKey"/>
        /// </summary>
        public bool UseXForwardedFor { get; set; } = false; //TODO: change this to true

        /// <summary>
        /// If <see cref="UserIdClaimType"/> is set to true then the value for the RemoteIp will be set to the value in the HTTP header with this key.
        /// If header value is null than RemoteIp is set to the value from request
        /// </summary>
        public string XForwardedForHeaderKey { get; set; } = "X-Forwarded-For";

        /// <summary>
        /// If this is null or empty current executing Assembly version is used
        /// </summary>
        public string AppVersion { get; set; }

        /// <summary>
        /// If values from <see cref="Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary"/> should be saved in audit log
        /// </summary>
        public bool AuditViewData { get; set; } = true;

        /// <summary>
        /// If bad request should be saved in audit log
        /// </summary>
        public bool AuditBadRequest { get; set; } = true;

        /// <summary>
        /// If all values that will be deleted by the database should be saved in audit log. If this is set to true it can affect application performance.
        /// </summary>
        public bool AuditCascadeDelete { get; set; } = true;

        /// <summary>
        /// Default audit subject name.
        /// </summary>
        public string DefaultSubjectName { get; set; } = "Audit";
    }
}
