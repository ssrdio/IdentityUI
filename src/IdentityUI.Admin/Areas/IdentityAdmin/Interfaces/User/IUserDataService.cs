using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Dashboard;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.User;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.User
{
    public interface IUserDataService
    {
        Result<DataTableResult<UserListViewModel>> GetAll(DataTableRequest request);
        Result<UserDetailsViewModel> GetDetailsViewModel(string id);
        Result<UserCredentialsViewModel> GetCredetialsViewModel(string id);
        Result<UserRolesViewModel> GetRolesViewModel(string id);
        Result<RoleViewModel> GetRoles(string id);

        Result<UserMenuViewModel> GetUserMenuViewModel(string id);

        Result<List<RegistrationsViewModel>> GetRegistrationStatistics(DateTimeOffset from, DateTimeOffset to);
        StatisticsViewModel GetStatistics();

        Result<UserSessionViewModel> GetUserSessionViewModel(string userId);
        Result<DataTableResult<SessionViewModel>> GetSessions(string userId, DataTableRequest request);
    }
}
