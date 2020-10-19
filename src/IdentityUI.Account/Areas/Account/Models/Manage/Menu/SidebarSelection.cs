using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Account.Areas.Account.Models.Manage.Menu
{
    public static class SidebarSelection
    {
        public static SidebarOptions SelectedOption { get; set; }
    }

    public enum SidebarOptions
    {
        Profile = 1,
        Credentials = 2,
        TwoFactorAuthenticator = 3,
        Audit = 4,
    }
}
