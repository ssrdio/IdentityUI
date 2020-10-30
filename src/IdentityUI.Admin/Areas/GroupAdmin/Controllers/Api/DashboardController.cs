using Microsoft.AspNetCore.Mvc;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Controllers.Api
{
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
    }
}
