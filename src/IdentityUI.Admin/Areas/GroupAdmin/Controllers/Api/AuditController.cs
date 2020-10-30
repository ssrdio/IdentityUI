using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.AdminUI.Template.Models.Select2;
using SSRD.Audit.Data;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Audit;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Services.Identity;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Controllers.Api
{
    public class AuditController : GroupAdminApiBaseController
    {
        private readonly IGroupAdminAuditDataService _groupAdminAuditDataService;
        private readonly IdentityUIClaimOptions _identityUIClaimOptions;

        public AuditController(IGroupAdminAuditDataService groupAdminAuditDataService, IOptions<IdentityUIClaimOptions> identityUIClaimOptions)
        {
            _groupAdminAuditDataService = groupAdminAuditDataService;
            _identityUIClaimOptions = identityUIClaimOptions.Value;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTableRequest"></param>
        /// <param name="actionType"></param>
        /// <param name="from">From must be in UTC time zone</param>
        /// <param name="to">To must be in UTC time zone</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<GroupAdminAuditTableModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] DataTableRequest dataTableRequest, [FromQuery] GroupAdminAuditTableRequest auditTableRequest)
        {
            Result<DataTableResult<GroupAdminAuditTableModel>> result = await _groupAdminAuditDataService.Get(
                User.GetGroupId(_identityUIClaimOptions),
                dataTableRequest,
                auditTableRequest);

            return result.ToApiResult();
        }

        [HttpGet("{auditId}")]
        [ProducesResponseType(typeof(GroupAdminAuditDetailsModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] long auditId)
        {
            Result<GroupAdminAuditDetailsModel> result = await _groupAdminAuditDataService.Get(User.GetGroupId(_identityUIClaimOptions), auditId);

            return result.ToApiResult();
        }

        [HttpGet]
        [ProducesResponseType(typeof(Select2Result<Select2Item>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetObjectTypes([FromQuery] Select2Request select2Request)
        {
            Result<Select2Result<Select2Item>> result = await _groupAdminAuditDataService.GetObjectTypes(User.GetGroupId(_identityUIClaimOptions),select2Request);

            return result.ToApiResult();
        }

        [HttpGet]
        [ProducesResponseType(typeof(Select2Result<Select2Item>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetObjectIdentifiers([FromQuery] Select2Request select2Request, [FromQuery] string objectType)
        {
            Result<Select2Result<Select2Item>> result = await _groupAdminAuditDataService.GetObjectIdentifiers(
                User.GetGroupId(_identityUIClaimOptions),
                select2Request,
                objectType);

            return result.ToApiResult();
        }

        [HttpGet]
        [ProducesResponseType(typeof(Select2Result<Select2Item>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSubjectIdentifiers([FromQuery] Select2Request select2Request, [FromQuery] SubjectTypes? subjectType)
        {
            Result<Select2Result<Select2Item>> result = await _groupAdminAuditDataService.GetSubjectIdentifiers(
                User.GetGroupId(_identityUIClaimOptions),
                select2Request,
                subjectType);

            return result.ToApiResult();
        }

        [HttpGet]
        [ProducesResponseType(typeof(Select2Result<Select2Item>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetResourceNames([FromQuery] Select2Request select2Request)
        {
            Result<Select2Result<Select2Item>> result = await _groupAdminAuditDataService.GetResourceNames(
                User.GetGroupId(_identityUIClaimOptions),
                select2Request);

            return result.ToApiResult();
        }
    }
}
