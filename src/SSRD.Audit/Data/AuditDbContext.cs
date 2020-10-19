using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SSRD.Audit.Models;
using SSRD.Audit.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SSRD.Audit.Data
{
    public class AuditDbContext : DbContext, IAuditDbContext
    {
        public DbSet<AuditEntity> Audit { get; set; }

        private readonly IAuditSubjectDataService _auditSubjectDataService;
        private readonly AuditOptions _auditOptions;

        public AuditDbContext(DbContextOptions<AuditDbContext> options, IAuditSubjectDataService auditSubjectDataService, IOptions<AuditOptions> auditOptions) : base(options)
        {
            _auditSubjectDataService = auditSubjectDataService;
            _auditOptions = auditOptions.Value;
        }

        public override int SaveChanges()
        {
            return SaveChanges(acceptAllChangesOnSuccess: true);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ProccessChangeTrackerResult proccessChangeTrackerResult = ChangeTrackerAuditService.ProccessChangeTracker(ChangeTracker, _auditOptions);
            AuditSubjectData auditSubjectData = _auditSubjectDataService.Get();

            IEnumerable<AuditEntity> auditEntities = proccessChangeTrackerResult.AuditObjectData
                .Select(x => new AuditEntity(
                    auditObjectData: x,
                    auditSubjectData: auditSubjectData));

            Audit.AddRange(auditEntities);

            return base.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return SaveChangesAsync(acceptAllChangesOnSuccess: true);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return SaveChangesAsync(acceptAllChangesOnSuccess: true, cancellationToken);
        }

        public async override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ProccessChangeTrackerResult proccessChangeTrackerResult = await ChangeTrackerAuditService.ProccessChangeTrackerAsync(ChangeTracker, _auditOptions);
            AuditSubjectData auditSubjectData = _auditSubjectDataService.Get();

            IEnumerable<AuditEntity> auditEntities = proccessChangeTrackerResult.AuditObjectData
                .Select(x => new AuditEntity(
                    auditObjectData: x,
                    auditSubjectData: auditSubjectData));

            Audit.AddRange(auditEntities);

            return await base.SaveChangesAsync();
        }
    }
}
