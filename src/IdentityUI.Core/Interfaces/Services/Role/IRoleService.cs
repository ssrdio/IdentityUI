using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Role.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services
{
    public interface IRoleService
    {
        Task<Result<string>> AddRole(NewRoleRequest newRoleRequest, string adminId);
        Result EditRole(string id, EditRoleRequest editRoleRequest, string adminId);

        Task<Result> Remove(string id);
    }
}
