using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Dashboard;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Dashboard;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces
{
    public interface IGroupAdminDashboardService
    {
        Task<GroupedStatisticsViewModel> GetIndexViewModel();
        Task<Result<List<RegistrationsViewModel>>> GetRegistrationStatistics(DateTimeOffset from, DateTimeOffset to);

        Task<Result<List<RegistrationsViewModel>>> GetActivityStatistics(DateTimeOffset from, DateTimeOffset to);
    }
}
