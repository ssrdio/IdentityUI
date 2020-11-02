using Microsoft.AspNetCore.Mvc;

namespace SSRD.AdminUI.Template.Views.Shared.Components.AdminUIDotLoader
{
    public class AdminUIDotLoaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
