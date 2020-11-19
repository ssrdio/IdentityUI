using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services.Group
{
    public interface IAddGroupUserFilter
    {
        Task<Result> BeforeAdd(string userid, string groupId, string roleId);
        Task<Result> AfterAdded(GroupUserEntity groupUser);
    }
}
