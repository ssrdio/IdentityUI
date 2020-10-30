using Microsoft.AspNetCore.Mvc;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Invite;
using SSRD.IdentityUI.Admin.Interfaces;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Controllers.Mvc
{
    public class InviteController : GroupAdminBaseController
    {
        private readonly IGroupInviteDataService _groupInviteDataService;

        public InviteController(IGroupInviteDataService groupInviteDataService)
        {
            _groupInviteDataService = groupInviteDataService;
        }

        public async Task<IActionResult> Index()
        {
            Result<GroupAdminInviteViewModel> result = await _groupInviteDataService.GetGroupAdminInviteViewModel();
            if(result.Failure)
            {
                return NotFoundView();
            }

            return View(result.Value);
        }
    }
}
