using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Data.Repository
{
    public interface IUserRepository : IBaseRepository<AppUserEntity>
    {
        List<GroupedCountData> GetRegistrations(DateTimeOffset from, DateTimeOffset to);

        int GetUsersCount();
        int GetActiveUsersCount();
        int GetUnconfirmedUsersCount();
        int GetDisabledUsersCount();
    }
}
