﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.AdminUI.Template.Models.Select2;
using SSRD.Audit.Attributes;
using SSRD.Audit.Data;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Audit;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Services.Audit;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/[area]/[controller]/[action]")]
    [Produces("application/json")]
    public class AuditController : BaseController
    {
        private readonly IAuditDataService _auditDataService;
        private readonly IAuditService _auditService;

        public AuditController(IAuditDataService auditDataService, IAuditService auditService)
        {
            _auditDataService = auditDataService;
            _auditService = auditService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("/[area]/[controller]/")]
        [AuditIgnore]
        public IActionResult Index()
        {
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
        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<AuditAdminTableModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromQuery] DataTableRequest dataTableRequest, [FromQuery] AuditTableRequest auditTableRequest)
        {
            Result<DataTableResult<AuditAdminTableModel>> result = await _auditDataService.Get(dataTableRequest, auditTableRequest);

            return result.ToApiResult();
        }

        [HttpGet("{auditId}")]
        [ProducesResponseType(typeof(AuditAdminDetailsModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromRoute] long auditId)
        {
            Result<AuditAdminDetailsModel> result = await _auditDataService.Get(auditId);

            return result.ToApiResult();
        }

        [HttpGet]
        [ProducesResponseType(typeof(Select2Result<Select2Item>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetObjectTypes([FromQuery] Select2Request select2Request)
        {
            Result<Select2Result<Select2Item>> result = await _auditDataService.GetObjectTypes(select2Request);

            return result.ToApiResult();
        }

        [HttpGet]
        [ProducesResponseType(typeof(Select2Result<Select2Item>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetObjectIdentifiers([FromQuery] Select2Request select2Request, [FromQuery] string objectType)
        {
            Result<Select2Result<Select2Item>> result = await _auditDataService.GetObjectIdentifiers(select2Request, objectType);

            return result.ToApiResult();
        }

        [HttpGet]
        [ProducesResponseType(typeof(Select2Result<Select2Item>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSubjectIdentifiers([FromQuery] Select2Request select2Request, [FromQuery] SubjectTypes? subjectType)
        {
            Result<Select2Result<Select2Item>> result = await _auditDataService.GetSubjectIdentifiers(select2Request, subjectType);

            return result.ToApiResult();
        }

        [HttpGet]
        [ProducesResponseType(typeof(Select2Result<Select2Item>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetResourceNames([FromQuery] Select2Request select2Request)
        {
            Result<Select2Result<Select2Item>> result = await _auditDataService.GetResourceNames(select2Request);

            return result.ToApiResult();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(List<>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetComments([FromRoute] long id)
        {
            Result<List<AuditCommentModel>> result = await _auditDataService.GetComments(id);

            return result.ToApiResult();
        }

        [HttpPost("{id}")]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddComment([FromRoute] long id, [FromBody] AddAuditCommentModel addAuditCommentModel)
        {
            Result result = await _auditService.AddComment(id, addAuditCommentModel);

            return result.ToApiResult();
        }

        [HttpGet]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Export([FromQuery] AuditTableRequest auditTableRequest)
        {
            Result<List<AuditExportModel>> result = await _auditDataService.GetExportData(auditTableRequest);

            return await JsonFile(result, $"audit_export_{DateTime.Now:yyyy-MM-dd}.json");
        }
    }
}
