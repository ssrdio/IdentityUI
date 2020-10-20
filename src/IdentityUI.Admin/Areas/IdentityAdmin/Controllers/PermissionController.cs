using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Permission;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Role;
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
        public IActionResult Details(string id)
        {
            Result<PermissionViewModel> result = _permissionDataService.GetViewModel(id);
            if (result.Failure)
            {
                return NotFoundView();
            }

            return View(result.Value);
        }

        [HttpGet]
        public IActionResult Roles(string id)
        {
            Result<PermissionMenuViewModel> result = _permissionDataService.GetMenuViewModel(id);
            if (result.Failure)
            {
                return NotFoundView();
            }

            return View(result.Value);

        }

        [HttpPost]
        public IActionResult Details(string id, EditPermissionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return NotFoundView();
            }

            Result<PermissionViewModel> role = _permissionDataService.GetViewModel(id);
            if (role.Failure)
            {
                return NotFoundView();
            }

            Result editResult = _permissionService.Edit(id, request);
            if (editResult.Failure)
            {
                ModelState.AddErrors(editResult.Errors);
                role.Value.StatusAlert = StatusAlertViewExtension.Get(editResult);

                return View(role.Value);
            }

            role.Value.StatusAlert = StatusAlertViewExtension.Get("Permission updated");
            return View(role.Value);
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

        [HttpPost]
        [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult Remove([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            Result result = _permissionService.Remove(id);
            if (result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }

        [HttpGet]
        [ProducesResponseType(typeof(DataTableResult<RoleListViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public IActionResult GetRoles([FromRoute] string id, DataTableRequest dataTableRequest)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result<DataTableResult<RoleListViewModel>> result = _permissionDataService.GetRoles(id, dataTableRequest);
            if(result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(result.Value);
        }
    }
}
