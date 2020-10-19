using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.Audit.Data
{
    public interface IAuditDbContext
    {
        DbSet<AuditEntity> Audit { get; set; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
