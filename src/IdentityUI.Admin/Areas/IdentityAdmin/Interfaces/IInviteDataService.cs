using SSRD.AdminUI.Template.Models.Select2;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Invite;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces
{
    public interface IInviteDataService
    {
        Result<DataTableResult<InviteTableModel>> Get(DataTableRequest dataTableRequest);

        Result<Select2Result<Select2ItemBase>> GetGlobalRoles(Select2Request select2Request);
        Result<Select2Result<Select2ItemBase>> GetGroups(Select2Request select2Request);
        Result<Select2Result<Select2ItemBase>> GetGroupRoles(Select2Request select2Request);
    }
}
