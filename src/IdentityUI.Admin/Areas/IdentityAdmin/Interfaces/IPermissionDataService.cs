using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Permission;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces
{
    public interface IPermissionDataService
    {
        Result<DataTableResult<PermissionTableModel>> Get(DataTableRequest dataTableRequest);
        Result<PermissionViewModel> GetViewModel(string id);
    }
}
