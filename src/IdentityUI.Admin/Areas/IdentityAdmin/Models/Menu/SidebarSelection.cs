using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Menu
{
    public static class SidebarSelection
    {
        public static SidebarOptions SelectedOption { get; set; }
    }

    public enum SidebarOptions
    {
        Dashboard = 1,
        User = 2,
        Role = 3,
        Permission = 4,
        Group = 5,
        Invite = 6,
        Setting = 7,
        Audit = 8,
    }
}
