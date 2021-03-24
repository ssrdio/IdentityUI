using SSRD.AdminUI.Template.Models;
using System.Collections.Generic;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect.Models
{
    public class ClientScopesModel
    {
        public List<DragAndDropItem> Available { get; set; }
        public List<DragAndDropItem> Assigned { get; set; }

        public ClientScopesModel(List<DragAndDropItem> available, List<DragAndDropItem> assigned)
        {
            Available = available;
            Assigned = assigned;
        }
    }
}
