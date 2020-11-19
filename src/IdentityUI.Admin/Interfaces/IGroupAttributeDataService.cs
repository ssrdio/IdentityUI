using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Models.Group;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Interfaces
{
    public interface IGroupAttributeDataService
    {
        Task<Result<DataTableResult<GroupAttributeTableModel>>> Get(string groupId, DataTableRequest dataTableRequest);
    }
}
