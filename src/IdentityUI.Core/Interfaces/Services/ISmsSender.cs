using SSRD.IdentityUI.Core.Models.Result;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services
{
    public interface ISmsSender
    {
        Task<Result> Send(string to, string message);
    }
}
