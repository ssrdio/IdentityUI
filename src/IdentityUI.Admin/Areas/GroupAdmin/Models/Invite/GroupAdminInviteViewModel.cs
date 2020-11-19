using SSRD.IdentityUI.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Invite
{
    public class GroupAdminInviteViewModel : GroupAdminViewModel
    {
        public List<RoleListData> CanAssignRoles { get; set; }

        public GroupAdminInviteViewModel(string groupId, List<RoleListData> canAssignRoles) : base(groupId)
        {
            CanAssignRoles = canAssignRoles;
        }
    }
}
