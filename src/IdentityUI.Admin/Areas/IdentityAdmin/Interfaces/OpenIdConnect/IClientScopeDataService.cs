using SSRD.AdminUI.Template.Models.DataTables;
using SSRD.AdminUI.Template.Models.Select2;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.OpenIdConnect.Models;
using SSRD.IdentityUI.Core.Data.Entities.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.OpenIdConnect
{
    public interface IClientScopeDataService
    {
        Task<Result<DataTableResult<ClientScopeTableModel>>> Get(DataTableRequest dataTableRequest);
        Task<Result<Select2Result<Select2Item>>> Get(Select2Request select2Request);
        Task<Result<ClientScopeDetailsModel>> Get(string id);
        Task<Result<ClientScopeDetailsModel>> Get(ClientScopeEntity clientScope);
    }
}
