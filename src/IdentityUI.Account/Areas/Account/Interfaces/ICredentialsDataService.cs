using SSRD.IdentityUI.Account.Areas.Account.Models.Manage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Account.Areas.Account.Interfaces
{
    public interface ICredentialsDataService
    {
        Task<CredentailsViewModel> GetViewModel();
    }
}
