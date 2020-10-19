using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.Audit.Attributes;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Account.Areas.Account.Interfaces;
using SSRD.IdentityUI.Account.Areas.Account.Models.Audit;
using SSRD.IdentityUI.Core.Models.Options;

namespace SSRD.IdentityUI.Account.Areas.Account.Controllers
{
    public class AuditController : BaseController
    {
        private readonly IAuditDataService _auditDataService;
        private readonly IdentityUIEndpoints _identityUIEndpoints;

        public AuditController(IAuditDataService auditDataService, IOptions<IdentityUIEndpoints> identityOptions)
        {
            _auditDataService = auditDataService;

            _identityUIEndpoints = identityOptions.Value;
        }

        [HttpGet]
        [AuditIgnore]
        public IActionResult Index()
        {
            if(!_identityUIEndpoints.ShowAuditToUser)
            {
                return NotFound();
            }

            AuditIndexViewModel auditIndexViewModel = _auditDataService.GetIndexViewModel();

            return View(auditIndexViewModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTableRequest"></param>
        /// <param name="actionType"></param>
        /// <param name="from">From must be in UTC time zone</param>
        /// <param name="to">To must be in UTC time zone</param>
        /// <returns></returns>
        [HttpGet("[area]/[controller]/[action]")]
        [ProducesResponseType(typeof(DataTableResult<AuditTableModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromQuery] DataTableRequest dataTableRequest, [FromQuery] AuditTableRequest auditTableRequest)
        {
            if (!_identityUIEndpoints.ShowAuditToUser)
            {
                return NotFound();
            }

            Result<DataTableResult<AuditTableModel>> result = await _auditDataService.Get(dataTableRequest, auditTableRequest);

            return result.ToApiResult();
        }

        [HttpGet("[area]/[controller]/[action]/{auditId}")]
        [ProducesResponseType(typeof(AuditDetailsModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromRoute] long auditId)
        {
            if (!_identityUIEndpoints.ShowAuditToUser)
            {
                return NotFound();
            }

            Result<AuditDetailsModel> result = await _auditDataService.Get(auditId);

            return result.ToApiResult();
        }
    }
}
