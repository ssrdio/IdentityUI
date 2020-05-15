using SSRD.AdminUI.Template.Models.Select2;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Group;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.Group
{
    public interface IGroupDataService
    {
        Result<DataTableResult<GroupTableModel>> Get(DataTableRequest dataTableRequest);
        
        Result<GroupMenuViewModel> GetMenuViewModel(string id);
        Result<GroupUserViewModel> GetGroupUserViewModel(string groupId);
        Result<GroupInviteViewModel> GetInviteViewModel(string groupId);
    }
}
