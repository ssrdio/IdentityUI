using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Dashboard
{
    public class StatisticsViewModel
    {
        public int UsersCount { get; set; }
        public int ActiveUsersCount { get; set; }
        public int UnconfirmedUsersCount { get; set; }
        public int DisabledUsersCount { get; set; }

        public StatisticsViewModel(int usersCount, int activeUsersCount, int unconfirmedUsersCount, int disabledUsersCount)
        {
            UsersCount = usersCount;
            ActiveUsersCount = activeUsersCount;
            UnconfirmedUsersCount = unconfirmedUsersCount;
            DisabledUsersCount = disabledUsersCount;
        }
    }
}
