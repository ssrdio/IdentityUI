using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Role;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.Role
{
    public interface IRoleDataService
    {
        Result<DataTableResult<RoleListViewModel>> GetAll(DataTableRequest request);
        Result<RoleDetailViewModel> GetDetails(string id);
        Result<DataTableResult<UserTableModel>> GetGlobalUsers(string roleId, DataTableRequest request);
        Result<DataTableResult<UserTableModel>> GetGroupUsers(string roleId, DataTableRequest request);
        Result<RoleMenuViewModel> GetRoleMenuViewModel(string id);
        NewRoleViewModel GetNewRoleViewModel(Result result = null);
    }
}
