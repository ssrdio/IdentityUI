using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.User;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.User
{
    public interface IUserAttributeDataService
    {
        Task<Result<DataTableResult<UserAttributeTableModel>>> Get(string userId, DataTableRequest dataTableRequest);
    }
}
