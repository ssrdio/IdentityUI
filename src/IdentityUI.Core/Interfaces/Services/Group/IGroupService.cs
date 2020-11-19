using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Models;
using SSRD.IdentityUI.Core.Services.Group.Models;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services.Group
{
    public interface IGroupService
    {
        Models.Result.Result Add(AddGroupRequest addGroup);
        Models.Result.Result Remove(string id);

        Task<Result<IdStringModel>> AddAsync(AddGroupRequest addGroup);
        Task<Result> Update(string groupId, UpdateGroupModel updateGroupModel);
        Task<Result> RemoveAsync(string id);
    }
}
