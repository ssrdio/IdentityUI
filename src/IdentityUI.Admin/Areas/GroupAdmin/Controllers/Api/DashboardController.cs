using Microsoft.AspNetCore.Mvc;
using SSRD.AdminUI.Template.Models;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Attributes;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using SSRD.IdentityUI.Core.Interfaces.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Controllers.Api
{
    [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_ACCESS_DASHBOARD)]
    public class DashboardController : GroupAdminApiBaseController
    {
        private readonly IGroupAdminDashboardService _groupAdminDashboardService;

        private readonly IIdentityUIUserInfoService _identityUIUserInfoService;

        public DashboardController(
            IGroupAdminDashboardService groupAdminDashboardService,
            IIdentityUIUserInfoService identityUIUserInfoService)
        {
            _groupAdminDashboardService = groupAdminDashboardService;
            _identityUIUserInfoService = identityUIUserInfoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRegistrationStatistics([FromRoute] string groupId, [FromQuery] TimeRangeRequest timeRangeRequest)
        {
            Result<List<TimeRangeStatisticsModel>> result = await _groupAdminDashboardService.GetRegistrationStatistics(
                groupId,
                timeRangeRequest);

            return result.ToApiResult();
        }

        [HttpGet]
        public async Task<IActionResult> GetActivityStatistics([FromRoute] string groupId, [FromQuery] TimeRangeRequest timeRangeRequest)
        {
            Result<List<TimeRangeStatisticsModel>> result = await _groupAdminDashboardService.GetActivityStatistics(
                groupId,
                timeRangeRequest);

            return result.ToApiResult();
        }
    }
}
