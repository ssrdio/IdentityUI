using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Interfaces.Services.Group;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.Group
{
    public class NullAddGroupUserFilter : IAddGroupUserFilter
    {
        public Task<Result> BeforeAdd(string userid, string groupId, string roleId)
        {
            return Task.FromResult(Result.Ok());
        }

        public Task<Result> AfterAdded(GroupUserEntity groupUser)
        {
            return Task.FromResult(Result.Ok());
        }
    }
}
