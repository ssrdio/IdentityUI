using Microsoft.AspNetCore.Mvc;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Attributes;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Audit;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Dashboard;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Invite;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.User;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Controllers
{
    public class GroupAdminViewController : GroupAdminBaseController
    {
        private readonly IGroupAdminAuditDataService _groupAdminAuditDataService;
        private readonly IGroupAdminDashboardService _groupAdminDashboardService;
        private readonly IGroupAdminInviteDataService _groupAdminInviteDataService;
        private readonly IGroupAdminUserDataService _groupAdminUserDataService;

        public GroupAdminViewController(
            IGroupAdminAuditDataService groupAdminAuditDataService,
            IGroupAdminDashboardService groupAdminDashboardService,
            IGroupAdminInviteDataService groupAdminInviteDataService,
            IGroupAdminUserDataService groupAdminUserDataService)
        {
            _groupAdminAuditDataService = groupAdminAuditDataService;
            _groupAdminDashboardService = groupAdminDashboardService;
            _groupAdminInviteDataService = groupAdminInviteDataService;
            _groupAdminUserDataService = groupAdminUserDataService;
        }

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_VIEW_AUDIT)]
        [HttpGet("/[area]/[action]")]
        public IActionResult Audit()
        {
            AuditIndexViewModel viewModel = _groupAdminAuditDataService.GetIndexViewModel();

            return View(viewModel);
        }

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_ACCESS_DASHBOARD)]
        [HttpGet("/[area]")]
        [HttpGet("/[area]/[action]")]
        public async Task<IActionResult> Dashboard()
        {
            GroupedStatisticsViewModel viewModel = await _groupAdminDashboardService.GetIndexViewModel();

            return View(viewModel);
        }

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_MANAGE_INVITES)]
        [HttpGet("/[area]/[action]")]
        public async Task<IActionResult> Invite()
        {
            Result<GroupAdminInviteViewModel> result = await _groupAdminInviteDataService.GetInviteViewModel();
            if (result.Failure)
            {
                return NotFoundView();
            }

            return View(result.Value);
        }

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_SEE_USERS)]
        [HttpGet("/[area]/User")]
        public async Task<IActionResult> Users()
        {
            Result<GroupAdminUserIndexViewModel> result = await _groupAdminUserDataService.GetIndexViewModel();
            if (result.Failure)
            {
                return NotFoundView();
            }

            return View(result.Value);
        }

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_ACCESS_USER_DETAILS)]
        [HttpGet("/[area]/User/Details/{groupUserId}")]
        public async Task<IActionResult> UserDetails([FromRoute] long groupUserId)
        {
            Result<GroupAdminUserDetailsViewModel> result = await _groupAdminUserDataService.GetDetailsViewModel(groupUserId);
            if (result.Failure)
            {
                return NotFoundView();
            }

            return View(result.Value);
        }
    }
}
