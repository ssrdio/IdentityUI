using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Role.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Interfaces.Services.Role
{
    public interface IRolePermissionService
    {
        Result Add(string roleId, AddRolePermissionRequest addRolePermission);
        Result Remove(long permissionRoleId);
        Result Remove(string roleId, string permissionId);
    }
}
