using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.User.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services
{
    public interface IAddUserService
    {
        Task<Result<string>> AddUser(NewUserRequest newUserRequest, string adminId);
        Task<Result> Register(RegisterRequest registerRequest);
    }
}
