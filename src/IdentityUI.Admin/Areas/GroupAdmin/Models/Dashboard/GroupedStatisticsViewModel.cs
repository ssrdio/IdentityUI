using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Dashboard
{
    public class GroupedStatisticsViewModel
    {
        public int UsersCount { get; set; }
        public int ActiveUsersCount { get; set; }
        public int UnconfirmedUsersCount { get; set; }
        public int DisabledUsersCount { get; set; }

        public GroupedStatisticsViewModel(
            int usersCount,
            int activeUsersCount,
            int unconfirmedUsersCount,
            int disabledUsersCount)
        {
            UsersCount = usersCount;
            ActiveUsersCount = activeUsersCount;
            UnconfirmedUsersCount = unconfirmedUsersCount;
            DisabledUsersCount = disabledUsersCount;
        }
    }
}
