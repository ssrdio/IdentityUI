using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Interfaces.Data;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Repository
{
    internal class UserRoleRepository : BaseRepository<UserRoleEntity>, IUserRoleRepository
    {
        public UserRoleRepository(IdentityDbContext context) : base(context)
        {

        }
    }
}
