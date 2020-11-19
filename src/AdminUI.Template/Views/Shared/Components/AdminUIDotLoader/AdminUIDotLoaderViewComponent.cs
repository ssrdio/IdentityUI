using Microsoft.AspNetCore.Mvc;

namespace SSRD.AdminUI.Template.Views.Shared.Components.AdminUIDotLoader
{
    public class AdminUIDotLoaderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(bool showLoader = false)
        {
            ViewModel viewModel = new ViewModel(
                showLoader: showLoader);

            return View(viewModel);
        }

        public class ViewModel
        {
            public bool ShowLoader { get; set; }

            public ViewModel(bool showLoader)
            {
                ShowLoader = showLoader;
            }
        }
    }
}
