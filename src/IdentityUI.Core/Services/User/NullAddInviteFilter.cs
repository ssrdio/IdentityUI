using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Interfaces.Services;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.User
{
    public class NullAddInviteFilter : IAddInviteFilter
    {
        public Task<Result> AfterAdded(InviteEntity invite)
        {
            return Task.FromResult(Result.Ok());
        }

        public Task<Result> BeforeAdd(string email, string roleId, string groupId, string groupRoleId)
        {
            return Task.FromResult(Result.Ok());
        }
    }
}
