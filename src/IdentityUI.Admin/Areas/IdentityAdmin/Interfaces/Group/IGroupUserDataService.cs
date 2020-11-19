using SSRD.AdminUI.Template.Models.Select2;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Group;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.Group
{
    [Obsolete("Use SSRD.IdentityUI.Admin.Interfaces.IGroupUserDataService")]
    public interface IGroupUserDataService
    {
        Result<DataTableResult<GroupUserTableModel>> Get(string groupId, DataTableRequest dataTableRequest);
        Result<Select2Result<Select2ItemBase>> GetAvailable(Select2Request select2Request);
    }
}
