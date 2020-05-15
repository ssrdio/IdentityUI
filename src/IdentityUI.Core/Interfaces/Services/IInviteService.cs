using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.User.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services
{
    public interface IInviteService
    {
        Task<Result> InviteToGroup(string groupId, InviteToGroupRequest inviteToGroupRequest);
        Task<Result> Invite(InviteRequest inviteRequest);
        Result Remove(string id);
    }
}
