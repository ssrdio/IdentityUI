using SSRD.IdentityUI.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Data.Repository
{
    public interface ISessionRepository : IBaseRepository<SessionEntity>
    {
        Task<bool> Remove(List<SessionEntity> sessions);
    }
}
