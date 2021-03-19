using SSRD.AdminUI.Template.Models.Select2;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect.Models
{
    public class ClientConsentViewModel
    {
        public List<Select2Item> Statuses { get; set; }

        public ClientMenuViewModel ClientMenu { get; set; }

        public ClientConsentViewModel(List<Select2Item> statuses, ClientMenuViewModel clientMenu)
        {
            Statuses = statuses;
            ClientMenu = clientMenu;
        }
    }
}
