using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Services.Group.Models;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services.Group
{
    public interface IGroupRegistrationService
    {
        Task<Result> Add(RegisterGroupModel registerGroupModel);
    }
}
