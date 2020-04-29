using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Permission;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Permission.Models;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers
{
    public class PermissionController : BaseController
    {
        private readonly IPermissionDataService _permissionDataService;
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionDataService permissionDataService, IPermissionService permissionService)
        {
            _permissionDataService = permissionDataService;
            _permissionService = permissionService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<PermissionTableModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult Get([FromQuery] DataTableRequest dataTableRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result<DataTableResult<PermissionTableModel>> result = _permissionDataService.Get(dataTableRequest);
            if (result.Failure)
            {
                ModelState.AddErrors(result.Errors);
                return BadRequest(ModelState);
            }

            return Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult Add([FromBody] AddPermissionRequest addPermission)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result result = _permissionService.Add(addPermission);
            if (result.Failure)
            {
                ModelState.AddErrors(result.Errors);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }
    }
}
