using SSRD.AdminUI.Template.Models.Select2;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect.Models
{
    public class ClientTokenViewModel : ClientMenuViewModel
    {
        public List<Select2Item> Statuses { get; set; }
        public List<Select2Item> Types { get; set; }

        public ClientTokenViewModel(string id, string name, List<Select2Item> statuses, List<Select2Item> types) : base(id, name)
        {
            Statuses = statuses;
            Types = types;
        }
    }
}
