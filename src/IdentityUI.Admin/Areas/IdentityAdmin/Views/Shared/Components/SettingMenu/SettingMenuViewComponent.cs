using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Views.Shared.Components.SettingMenu
{
    public class SettingMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ViewModel viewModel)
        {
            return View("Default", viewModel);
        }

        public class ViewModel
        {
            public string Name { get; set; }

            public TabSelected TabSelected { get; set; }


            public ViewModel(string name, TabSelected tabSelected)
            {
                Name = name;

                TabSelected = tabSelected;
            }
        }

        public enum TabSelected
        {
            Email = 1,
        }
    }
}
