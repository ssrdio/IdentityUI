using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect.Models
{
    public class ClientScopesModel
    {
        public IList<string> Available { get; set; }
        public IList<string> Assigned { get; set; }

        public ClientScopesModel(IList<string> available, IList<string> assigned)
        {
            Available = available;
            Assigned = assigned;
        }
    }
}
