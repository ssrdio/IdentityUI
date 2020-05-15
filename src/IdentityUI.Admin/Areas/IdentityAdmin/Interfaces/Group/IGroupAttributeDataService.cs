using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Group;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.Group
{
    public interface IGroupAttributeDataService
    {
        Result<DataTableResult<GroupAttributeTableModel>> Get(string groupId, DataTableRequest dataTableRequest);
    }
}
