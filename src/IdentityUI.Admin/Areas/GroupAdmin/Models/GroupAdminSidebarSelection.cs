using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models
{
    public class GroupAdminSidebarSelection
    {
        public static GroupAdminSidebarOptions SelectedOption { get; set; } = GroupAdminSidebarOptions.Dashboard;
    }

    public enum GroupAdminSidebarOptions
    {
        Dashboard = 1,
        User = 2,
        Invite = 3,
        Attributes = 4,
        Audit = 5,
    }
}
