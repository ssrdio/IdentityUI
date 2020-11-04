using SSRD.IdentityUI.Core.Data.Models;
using System.Collections.Generic;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.User
{
    public class GroupAdminUserIndexViewModel : GroupAdminViewModel
    {
        public List<RoleListData> CanMangeGroupRoles { get; set; }
        public List<RoleListData> CanAssigneGroupRoles { get; set; }

        public bool CanChangeOwnRole { get; set; }

        public GroupAdminUserIndexViewModel(string groupId,
            List<RoleListData> canMangeGroupRoles,
            List<RoleListData> canAssigneGroupRoles,
            bool canChangeOwnRole) : base(groupId)
        {
            GroupId = groupId;

            CanMangeGroupRoles = canMangeGroupRoles;
            CanAssigneGroupRoles = canAssigneGroupRoles;
            CanChangeOwnRole = canChangeOwnRole;
        }
    }
}
