using Microsoft.AspNetCore.Mvc;

namespace SSRD.IdentityUI.Admin.Views.Shared.Components.AddGroupAttributeModal
{
    public class AddGroupAttributeModalViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("Default");
        }
    }
}
