using SSRD.AdminUI.Template.Models;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Dashboard;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces
{
    public interface IGroupAdminDashboardService
    {
        Task<Result<GroupedStatisticsViewModel>> GetIndexViewModel(string groupId);

        Task<Result<List<TimeRangeStatisticsModel>>> GetRegistrationStatistics(string groupId, TimeRangeRequest timeRangeRequest);
        Task<Result<List<TimeRangeStatisticsModel>>> GetActivityStatistics(string groupId, TimeRangeRequest timeRangeRequest);
    }
}
