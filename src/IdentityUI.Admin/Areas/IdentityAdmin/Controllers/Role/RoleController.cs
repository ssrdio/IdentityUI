using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Role;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Role.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.Role;
using Microsoft.AspNetCore.Http;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers.Role
{
    public class RoleController : BaseController
    {
        private readonly IRoleDataService _roleDataService;
        private readonly IRoleService _roleService;

        public RoleController(IRoleDataService roleDataService, IRoleService roleService)
        {
            _roleDataService = roleDataService;
            _roleService = roleService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] DataTableRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result<DataTableResult<RoleListViewModel>> result = _roleDataService.GetAll(request);
            if (result.Failure)
            {
                ModelState.AddErrors(result.Errors);
                return View(ModelState);
            }

            return Ok(result.Value);
        }

        [HttpGet]
        public IActionResult Details(string id)
        {
            if (!ModelState.IsValid)
            {
                return NotFoundView();
            }

            Result<RoleDetailViewModel> result = _roleDataService.GetDetails(id);
            if (result.Failure)
            {
                return NotFoundView();
            }

            RoleDetailViewModel roleDetails = result.Value;

            return View(roleDetails);
        }

        [HttpPost]
        public IActionResult Details(string id, EditRoleRequest request)
        {
            if (!ModelState.IsValid)
            {
                return NotFoundView();
            }

            Result<RoleDetailViewModel> role = _roleDataService.GetDetails(id);
            if (role.Failure)
            {
                return NotFoundView();
            }

            Result editResult = _roleService.EditRole(id, request, GetUserId());
            if (editResult.Failure)
            {
                ModelState.AddErrors(editResult.Errors);
                role.Value.StatusAlert = StatusAlertViewExtension.Get(editResult);

                return View(role.Value);
            }

            role.Value.StatusAlert = StatusAlertViewExtension.Get("Role updated");
            return View(role.Value);
        }

        [HttpGet]
        public IActionResult Users(string id)
        {
            if (!ModelState.IsValid)
            {
                return NotFoundView();
            }

            Result<RoleMenuViewModel> result = _roleDataService.GetRoleMenuViewModel(id);
            if (result.Failure)
            {
                return NotFoundView();
            }

            return View(result.Value);
        }

        [HttpGet]
        public IActionResult New()
        {
            NewRoleViewModel newRoleViewModel = _roleDataService.GetNewRoleViewModel();

            return View(newRoleViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> New(NewRoleRequest newRoleRequest)
        {
            if (!ModelState.IsValid)
            {
                NewRoleViewModel newRoleViewModel = _roleDataService.GetNewRoleViewModel();

                return View(newRoleViewModel);
            }

            Result<string> result = await _roleService.AddRole(newRoleRequest, GetUserId());
            if (result.Failure)
            {
                ModelState.AddErrors(result.Errors);

                NewRoleViewModel newRoleViewModel = _roleDataService.GetNewRoleViewModel(result);
                return View(newRoleViewModel);
            }

            return RedirectToAction(nameof(Details), new { id = result.Value });
        }

        [HttpGet]
        public IActionResult Assignments(string id)
        {
            if (!ModelState.IsValid)
            {
                return NotFoundView();
            }

            Result<RoleMenuViewModel> result = _roleDataService.GetRoleMenuViewModel(id);
            if (result.Failure)
            {
                return NotFoundView();
            }

            return View(result.Value);
        }

        [HttpGet]
        public IActionResult Permissions(string id)
        {
            if (!ModelState.IsValid)
            {
                return NotFoundView();
            }

            Result<RoleMenuViewModel> result = _roleDataService.GetRoleMenuViewModel(id);
            if (result.Failure)
            {
                return NotFoundView();
            }

            return View(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(DataTableResult<RolePermissionTableModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IDictionary<string, string[]>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Remove([FromRoute] string id)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result result = await _roleService.Remove(id);
            if(result.Failure)
            {
                ModelState.AddErrors(result);
                return BadRequest(ModelState);
            }

            return Ok(new EmptyResult());
        }
    }
}
