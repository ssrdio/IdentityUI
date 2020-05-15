using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Views.Group.Components.GroupMenu;
using SSRD.IdentityUI.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Group
{
    public class GroupInviteViewModel
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }

        public List<RoleListData> CanAssignRoles { get; set; }

        public GroupInviteViewModel(string groupId, string groupName)
        {
            GroupId = groupId;
            GroupName = groupName;
        }

        public GroupMenuViewComponent.ViewModel ToViewComponent(GroupMenuViewComponent.TabSelected tabSelected)
        {
            return new GroupMenuViewComponent.ViewModel(
                id: GroupId,
                name: GroupName,
                tabSelected: tabSelected);
        }
    }
}
