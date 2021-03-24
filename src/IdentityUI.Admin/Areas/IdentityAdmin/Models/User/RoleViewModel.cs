using SSRD.AdminUI.Template.Models;
using System.Collections.Generic;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.User
{
    public class RoleViewModel
    {
        public List<DragAndDropItem> AssignedRoles { get; set; }
        public List<DragAndDropItem> AvailableRoles { get; set; }

        public RoleViewModel(List<DragAndDropItem> assignedRoles, List<DragAndDropItem> availableRoles)
        {
            AssignedRoles = assignedRoles;
            AvailableRoles = availableRoles;
        }
    }
}
