using SSRD.AdminUI.Template.Models.Select2;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Views.Group.Components.GroupMenu;
using SSRD.IdentityUI.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Group
{
    public class GroupUserViewModel
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }

        public List<RoleListData> CanMangeGroupRoles { get; set; }
        public List<RoleListData> CanAssigneGroupRoles { get; set; }

        public bool CanChangeOwnRole { get; set; }

        public GroupUserViewModel(string groupId, string groupName)
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
