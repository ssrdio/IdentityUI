using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Group.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services.Group
{
    public interface IGroupUserService
    {
        Result AddExisting(string groupId, AddExistingUserRequest addExistingUserRequest);

        Result ChangeRole(long groupUserId, string roleId, string userId);
        Result Remove(long groupUserId);
        Result Leave(string userId, string groupId);
    }
}
