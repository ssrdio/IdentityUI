using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Models;
using SSRD.IdentityUI.Core.Services.User.Models;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services
{
    public interface IAddUserService
    {
        Task<Models.Result.Result<string>> AddUser(NewUserRequest newUserRequest, string adminId);
        Task<Models.Result.Result> Register(RegisterRequest registerRequest);
        Task<Models.Result.Result> AcceptInvite(AcceptInviteRequest acceptInvite);
        Task<Models.Result.Result> ExternalLoginRequest(ExternalLoginRegisterRequest externalLoginRegisterRequest);

        Task<Result> UserExists(BaseRegisterRequest baseRegisterRequest);
        Task<Result<IdStringModel>> RegisterForGroup(GroupBaseUserRegisterRequest registerRequest);
    }
}
