using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Email.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services
{
    public interface IManageEmailService
    {
        Result Edit(long id, EditEmailRequest editEmail);
        Task<Result> TestEmail(long id, SendTesEmailRequest sendTesEmail);
    }
}
