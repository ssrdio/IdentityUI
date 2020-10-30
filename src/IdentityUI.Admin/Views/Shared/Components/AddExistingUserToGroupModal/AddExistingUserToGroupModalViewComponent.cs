using Microsoft.AspNetCore.Mvc;

namespace SSRD.IdentityUI.Admin.Views.Shared.Components.AddExistingUserToGroupModal
{
    public class AddExistingUserToGroupModalViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("Default");
        }
    }
}
