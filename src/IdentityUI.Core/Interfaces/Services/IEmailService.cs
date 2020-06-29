using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services
{
    public interface IEmailService
    {
        Task<Result> SendTest(string email, EmailEntity emailEntity);

        Task<Result> SendInvite(string email, string token);
        Task<Result> SendConfirmation(string email, string token);
        Task<Result> SendPasswordRecovery(string email, string token);
        Task<Result> SendPasswordWasReset(string email);

        Task<Result> Send2faToken(string email, string token);
    }
}
