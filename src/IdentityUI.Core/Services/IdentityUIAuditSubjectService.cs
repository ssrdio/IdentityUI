using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SSRD.Audit.Models;
using SSRD.Audit.Services;
using SSRD.IdentityUI.Core.Models;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Services.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services
{
    public class IdentityUIAuditSubjectService : HttpContextAuditDataService
    {
        private readonly IdentityUIClaimOptions _identityUIClaimOptions;

        public IdentityUIAuditSubjectService(
            IHttpContextAccessor httpContextAccessor,
            IOptions<AuditOptions> auditOptions,
            IOptions<IdentityUIClaimOptions> identityUIClaimOptions) : base(httpContextAccessor, auditOptions)
        {
            _identityUIClaimOptions = identityUIClaimOptions.Value;
        }

        public override string GetUserId()
        {
            return _httpContextAccessor.HttpContext.User.GetUserId(_identityUIClaimOptions);
        }

        public override string GetSubjectMetadata()
        {
            if(!_httpContextAccessor.HttpContext.User.IsImpersonized(_identityUIClaimOptions))
            {
                return null;
            }

            SubjectMetadataModel subjectMetadataModel = new SubjectMetadataModel(
                impersonatorId: _httpContextAccessor.HttpContext.User.GetImpersonatorId(_identityUIClaimOptions));

            string json;

#if NET_CORE2
            json = Newtonsoft.Json.JsonConvert.SerializeObject(subjectMetadataModel);
#elif NET_CORE3
            json = System.Text.Json.JsonSerializer.Serialize<SubjectMetadataModel>(subjectMetadataModel);
#endif

            return json;
        }

        public override string GetGroupIdentifier()
        {
            return _httpContextAccessor.HttpContext.User.GetGroupId(_identityUIClaimOptions);
        }
    }
}
