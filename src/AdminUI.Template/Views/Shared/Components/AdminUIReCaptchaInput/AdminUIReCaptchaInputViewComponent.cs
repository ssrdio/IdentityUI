using Microsoft.AspNetCore.Mvc;
using SSRD.AdminUI.Template.Models;

namespace SSRD.AdminUI.Template.Views.Shared.Components.AdminUIReCaptchaInput
{
    public class AdminUIReCaptchaInputViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
