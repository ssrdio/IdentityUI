using SSRD.IdentityUI.Account.Areas.Account.Models.Account;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Account.Areas.Account.Interfaces
{
    public interface IAccountDataService
    {
        Task<LoginViewModel> GetLoginViewModel(string returnUrl, string error = null);
        RegisterSuccessViewModel GetRegisterSuccessViewModel();
        RegisterViewModel GetRegisterViewModel();
        RegisterGroupViewModel GetRegisterGroupViewModel();

        Task<Result<ExternalLoginRegisterViewModel>> GetExternalLoginViewModel(string returnUrl);
    }
}
