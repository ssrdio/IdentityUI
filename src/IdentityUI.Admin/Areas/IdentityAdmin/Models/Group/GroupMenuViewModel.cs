using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Views.Group.Components.GroupMenu;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Group
{
    public class GroupMenuViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public GroupMenuViewModel(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public GroupMenuViewComponent.ViewModel ToViewComponent(GroupMenuViewComponent.TabSelected tabSelected)
        {
            return new GroupMenuViewComponent.ViewModel(
                id: Id,
                name: Name,
                tabSelected: tabSelected);
        }
    }
}
