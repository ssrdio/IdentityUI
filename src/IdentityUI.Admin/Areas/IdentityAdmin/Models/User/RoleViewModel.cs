using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.User
{
    public class RoleViewModel
    {
        public class RoleModel
        {
            public string Id { get; set; }
            public string Name { get; set; }

            public RoleModel(string id, string name)
            {
                Id = id;
                Name = name;
            }
        }
        [DisplayName("Assigned roles")]
        public List<RoleModel> AssignedRoles { get; set; }
        [DisplayName("Available roles")]
        public List<RoleModel> AvailableRoles { get; set; }

        public RoleViewModel(List<RoleModel> assignedRoles, List<RoleModel> availableRoles)
        {
            AssignedRoles = assignedRoles;
            AvailableRoles = availableRoles;
        }
    }
}
