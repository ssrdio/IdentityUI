using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Setting.Email;
using SSRD.IdentityUI.Core.Data.Enums.Entity;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.Setting
{
    public interface IEmailDataService
    {
        Result<DataTableResult<EmailTableModel>> Get(DataTableRequest dataTableRequest);

        EmailIndexViewModel GetIndexViewModel();
        Result<EmailViewModel> GetViewModel(long id, string userId);
    }
}
