using Microsoft.AspNetCore.Mvc;

namespace SSRD.AdminUI.Template.Views.Shared.Components.AdminUIReCaptchaJavaScript
{
    public class AdminUIReCaptchaJavaScriptViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string action = null)
        {
            ViewModel viewModel = new ViewModel(
                action: action);

            return View(viewModel);
        }

        public class ViewModel
        {
            public string Action { get; set; }

            public ViewModel(string action)
            {
                Action = action;
            }
        }
    }
}
