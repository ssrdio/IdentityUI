using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Views.Client.Components.ClientMenu
{
    public class ClientMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ViewModel viewModel)
        {
            return View(viewModel);
        }

        public class ViewModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public TabSelected TabSelected { get; set; }

            public ViewModel(string id, string name, TabSelected tabSelected)
            {
                Id = id;
                Name = name;
                TabSelected = tabSelected;
            }
        }

        public enum TabSelected
        {
            Details = 1,
            Credentials = 2,
            Scopes = 3,
            Consents = 4,
            Tokens = 5,
        }
    }
}
