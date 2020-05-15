using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Views.Group.Components.GroupMenu
{
    public class GroupMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ViewModel viewModel)
        {
            return View("Default", viewModel);
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
            Users,
            Invites,
            Attributes,
        }
    }
}
