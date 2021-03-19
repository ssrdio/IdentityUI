using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Services.Audit;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services
{
    public interface IAuditService
    {
        Task<Result> AddComment(long id, AddAuditCommentModel addAuditComment);
    }
}
