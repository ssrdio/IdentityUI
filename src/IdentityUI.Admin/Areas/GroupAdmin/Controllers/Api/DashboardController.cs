using Microsoft.AspNetCore.Mvc;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Attributes;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Dashboard;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Controllers.Api
{
    [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_ACCESS_DASHBOARD)]
    public class DashboardController : GroupAdminApiBaseController
    {
        private readonly IGroupAdminDashboardService _groupAdminDashboardService;

        public DashboardController(IGroupAdminDashboardService groupAdminDashboardService)
        {
            _groupAdminDashboardService = groupAdminDashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRegistrationStatistics([FromQuery] DateTimeOffset from, [FromQuery] DateTimeOffset to)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result<List<RegistrationsViewModel>> result = await _groupAdminDashboardService.GetRegistrationStatistics(from, to);

            return result.ToApiResult();
        }

        [HttpGet]
        public async Task<IActionResult> GetActivityStatistics([FromQuery] DateTimeOffset from, [FromQuery] DateTimeOffset to)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Result<List<RegistrationsViewModel>> result = await _groupAdminDashboardService.GetActivityStatistics(from, to);

            return result.ToApiResult();
        }
    }
}
