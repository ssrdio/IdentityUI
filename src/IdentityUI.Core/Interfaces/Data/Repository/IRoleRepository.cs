using SSRD.IdentityUI.Core.Data.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Interfaces.Data.Repository
{
    public interface IRoleRepository : IBaseRepository<RoleEntity>
    {
        List<RoleEntity> GetAssigned(string userId);
        List<RoleEntity> GetAvailable(string userId);
    }
}
