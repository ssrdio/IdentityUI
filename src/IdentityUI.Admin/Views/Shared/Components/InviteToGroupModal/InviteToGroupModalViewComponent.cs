using Microsoft.AspNetCore.Mvc;

namespace SSRD.IdentityUI.Admin.Views.Shared.Components.InviteToGroupModal
{
    public class InviteToGroupModalViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("Default");
        }
    }
}
