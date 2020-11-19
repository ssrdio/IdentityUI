using Microsoft.AspNetCore.Mvc;

namespace SSRD.AdminUI.Template.Views.Shared.Components.AdminUIReCaptchaJavaScript
{
    public class AdminUIReCaptchaJavaScriptViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string action = null, string formContainer = null)
        {
            ViewModel viewModel = new ViewModel(
                action: action,
                formContainer: formContainer);

            return View(viewModel);
        }

        public class ViewModel
        {
            public string Action { get; set; }
            public string FormContainer { get; set; }

            public ViewModel(string action, string formContainer)
            {
                Action = action;
                FormContainer = formContainer;
            }
        }
    }
}
