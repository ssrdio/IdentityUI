using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Account.Areas.Account.Models.Session;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Account.Areas.Account.Interfaces
{
    public interface ISessionDataService
    {
        Task<Result<DataTableResult<SessionModel>>> Get(DataTableRequest dataTableRequest);
    }
}
