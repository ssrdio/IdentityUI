using SSRD.AdminUI.Template.Models.Select2;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Role;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.Role
{
    public interface IRolePermissionsDataService
    {
        Result<DataTableResult<RolePermissionTableModel>> Get(string roleId, DataTableRequest dataTableRequest);
        Result<Select2Result<Select2ItemBase>> GetAvailable(string roleId, Select2Request select2Request);
    }
}
