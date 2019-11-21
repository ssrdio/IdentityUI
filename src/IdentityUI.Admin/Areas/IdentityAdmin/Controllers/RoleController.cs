using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Role;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.Role;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Role.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Controllers
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
                return RedirectToAction(nameof(Index));
            }

            Result<RoleDetailViewModel> result = _roleDataService.GetDetails(id);
            if (result.Failure)
            {
                ModelState.AddErrors(result.Errors);
                return RedirectToAction(nameof(Index));
            }

            return View(result.Value);
        }

        [HttpPost]
        public IActionResult Edit(string id, EditRoleRequest request)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            Result<RoleDetailViewModel> role = _roleDataService.GetDetails(id);
            if(role.Failure)
            {
                return RedirectToAction(nameof(Index));
            }

            Result editResult = _roleService.EditRole(id, request, GetUserId());
            if(editResult.Failure)
            {
                ModelState.AddErrors(editResult.Errors);
                role.Value.StatusAlert = StatusAlertViewExtension.Get(editResult);

                return View("Details", role.Value);
            }

            role.Value.StatusAlert = StatusAlertViewExtension.Get("Role updated");
            return View("Details", role.Value);
        }

        [HttpGet]
        public IActionResult Users(string id)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }

            Result<RoleUserViewModel> result = _roleDataService.GetUsersViewModel(id);
            if(result.Failure)
            {
                ModelState.AddErrors(result.Errors);
                return RedirectToAction(nameof(Index));
            }

            return View(result.Value);
        }

        [HttpGet]
        public IActionResult GetUsers([FromRoute] string id, [FromQuery] DataTableRequest dataTableRequest)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result<DataTableResult<UserViewModel>> result = _roleDataService.GetUsers(id, dataTableRequest);
            if(result.Failure)
            {
                ModelState.AddErrors(result.Errors);
                return BadRequest(ModelState);
            }

            return Ok(result.Value);
        }

        [HttpGet]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> New(NewRoleRequest newRoleRequest)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            Result<string> result = await _roleService.AddRole(newRoleRequest, GetUserId());
            if(result.Failure)
            {
                ModelState.AddErrors(result.Errors);

                return View(new NewRoleViewModel(StatusAlertViewExtension.Get(result)));
            }

            return RedirectToAction(nameof(Details), new { id = result.Value});
        }
    }
}
