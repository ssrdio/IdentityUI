using SSRD.IdentityUI.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Invite
{
    public class GroupAdminInviteViewModel
    {
        public List<RoleListData> CanAssignRoles { get; set; }

        public GroupAdminInviteViewModel(List<RoleListData> canAssignRoles)
        {
            CanAssignRoles = canAssignRoles;
        }
    }
}
