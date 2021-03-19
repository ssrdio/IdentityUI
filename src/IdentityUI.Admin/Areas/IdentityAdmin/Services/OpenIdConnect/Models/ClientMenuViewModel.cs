using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Views.Client.Components.ClientMenu;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect.Models
{
    public class ClientMenuViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public ClientMenuViewModel(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public ClientMenuViewComponent.ViewModel ToMenuViewModel(ClientMenuViewComponent.TabSelected tabSelected)
        {
            ClientMenuViewComponent.ViewModel viewModel = new ClientMenuViewComponent.ViewModel(
                id: Id,
                name: Name,
                tabSelected: tabSelected);

            return viewModel;
        }
    }
}
