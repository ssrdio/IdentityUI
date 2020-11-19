using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Data.Entities;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services
{
    public interface IAddInviteFilter
    {
        Task<Result> BeforeAdd(string email, string roleId, string groupId, string groupRoleId);
        Task<Result> AfterAdded(InviteEntity invite);
    }
}
