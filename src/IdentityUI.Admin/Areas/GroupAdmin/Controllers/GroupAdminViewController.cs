using Microsoft.AspNetCore.Mvc;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Attributes;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Attribute;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Audit;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Dashboard;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Invite;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Settings;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.User;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using SSRD.IdentityUI.Core.Services.Identity;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Controllers
{
    //[Route("/[area]/{groupId}/[action]")]
    public class GroupAdminViewController : GroupAdminBaseController
    {
        private readonly IGroupAdminAuditDataService _groupAdminAuditDataService;
        private readonly IGroupAdminDashboardService _groupAdminDashboardService;
        private readonly IGroupAdminInviteDataService _groupAdminInviteDataService;
        private readonly IGroupAdminUserDataService _groupAdminUserDataService;
        private readonly IGroupAdminAttributeDataService _groupAdminAttributeDataService;
        private readonly IGroupAdminSettingsDataService _groupAdminSettingsDataService;

        public GroupAdminViewController(
            IGroupAdminAuditDataService groupAdminAuditDataService,
            IGroupAdminDashboardService groupAdminDashboardService,
            IGroupAdminInviteDataService groupAdminInviteDataService,
            IGroupAdminUserDataService groupAdminUserDataService,
            IGroupAdminAttributeDataService groupAdminAttributeDataService,
            IGroupAdminSettingsDataService groupAdminSettingsDataService)
        {
            _groupAdminAuditDataService = groupAdminAuditDataService;
            _groupAdminDashboardService = groupAdminDashboardService;
            _groupAdminInviteDataService = groupAdminInviteDataService;
            _groupAdminUserDataService = groupAdminUserDataService;
            _groupAdminAttributeDataService = groupAdminAttributeDataService;
            _groupAdminSettingsDataService = groupAdminSettingsDataService;
        }

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_VIEW_AUDIT)]
        [HttpGet("/[area]/{groupId}/[action]")]
        public async Task<IActionResult> Audit([FromRoute] string groupId)
        {
            Result<AuditIndexViewModel> result = await _groupAdminAuditDataService.GetIndexViewModel(groupId);
            if(result.Failure)
            {
                return NotFoundView();
            }

            return View(result.Value);
        }

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_ACCESS_DASHBOARD)]
        [HttpGet("/[area]/{groupId}/[action]")]
        public async Task<IActionResult> Dashboard([FromRoute] string groupId)
        {
            Result<GroupedStatisticsViewModel> result = await _groupAdminDashboardService.GetIndexViewModel(groupId);
            if(result.Failure)
            {
                return NotFoundView();
            }

            return View(result.Value);
        }

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_MANAGE_INVITES)]
        [HttpGet("/[area]/{groupId}/[action]")]
        public async Task<IActionResult> Invite([FromRoute] string groupId)
        {
            Result<GroupAdminInviteViewModel> result = await _groupAdminInviteDataService.GetInviteViewModel(groupId);
            if (result.Failure)
            {
                return NotFoundView();
            }

            return View(result.Value);
        }

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_SEE_USERS)]
        [HttpGet("/[area]/{groupId}/User")]
        public async Task<IActionResult> Users([FromRoute] string groupId)
        {
            Result<GroupAdminUserIndexViewModel> result = await _groupAdminUserDataService.GetIndexViewModel(groupId);
            if (result.Failure)
            {
                return NotFoundView();
            }

            return View(result.Value);
        }

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_ACCESS_USER_DETAILS)]
        [HttpGet("/[area]/{groupId}/User/Details/{groupUserId}")]
        public async Task<IActionResult> UserDetails([FromRoute] string groupId, [FromRoute] long groupUserId)
        {
            Result<GroupAdminUserDetailsViewModel> result = await _groupAdminUserDataService.GetDetailsViewModel(groupId, groupUserId);
            if (result.Failure)
            {
                return NotFoundView();
            }

            return View(result.Value);
        }

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_MANAGE_ATTRIBUTES)]
        [HttpGet("/[area]/{groupId}/[action]")]
        public async Task<IActionResult> Attribute([FromRoute] string groupId)
        {
            Result<GroupAdminAttributeViewModel> result = await _groupAdminAttributeDataService.GetViewModel(groupId);
            if(result.Failure)
            {
                return NotFoundView();
            }

            return View(result.Value);
        }

        [GroupAdminAuthorize(IdentityUIPermissions.GROUP_CAN_MANAGE_SETTINGS)]
        [HttpGet("/[area]/{groupId}/[action]")]
        public async Task<IActionResult> Settings([FromRoute] string groupId)
        {
            Result<GroupAdminSettingsViewModel> result = await _groupAdminSettingsDataService.GetViewModel(groupId);
            if (result.Failure)
            {
                return NotFoundView();
            }

            return View(result.Value);
        }
    }
}
