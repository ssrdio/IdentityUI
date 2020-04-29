using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Role.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Interfaces.Services.Role
{
    public interface IRoleAssignmentService
    {
        Result Add(string roleId, AddRoleAssignmentRequest addRoleAssignment);
        Result Remove(string roleId, string assignedRoleId);
        Result Remove(long id);
    }
}
