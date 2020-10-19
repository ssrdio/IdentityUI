using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SSRD.IdentityUI.Core.Infrastructure.Data.Config.Identity;
using SSRD.IdentityUI.Core.Infrastructure.Data.Config;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Infrastructure.Data.Config.Group;
using SSRD.IdentityUI.Core.Data.Entities.User;
using SSRD.IdentityUI.Core.Infrastructure.Data.Config.User;
using Microsoft.EntityFrameworkCore.Storage;
using SSRD.Audit.Data;
using SSRD.Audit.Services;
using Microsoft.Extensions.DependencyInjection;
using SSRD.Audit.Models;
using Microsoft.Extensions.Logging;

namespace SSRD.IdentityUI.Core.Infrastructure.Data
{
    public class IdentityDbContext : IdentityDbContext<AppUserEntity, RoleEntity, string, UserClaimEntity, UserRoleEntity, UserLoginEntity, RoleClaimEntity, UserTokenEntity>, IAuditDbContext
    {
        public DbSet<SessionEntity> Sessions { get; set; }
        public DbSet<RoleAssignmentEntity> RoleAssignments { get; set; }
        public DbSet<PermissionEntity> Permissions { get; set; }
        public DbSet<PermissionRoleEntity> PermissionRole { get; set; }
        public DbSet<UserImageEntity> UserImage { get; set; }

        public DbSet<UserAttributeEntity> UserAttributes { get; set; }

        public DbSet<GroupAttributeEntity> GroupAttributes { get; set; }
        public DbSet<GroupEntity> Groups { get; set; }
        public DbSet<GroupUserEntity> GroupUsers { get; set; }

        public DbSet<InviteEntity> Invite { get; set; }
        public DbSet<EmailEntity> Emails { get; set; }
        public DbSet<AuditEntity> Audit { get; set; }

        private readonly IAuditSubjectDataService _auditDataService;
        private readonly ILogger<IdentityDbContext> _logger;

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options, IAuditSubjectDataService auditDataService, ILogger<IdentityDbContext> logger)
            : base(options)
        {
            _auditDataService = auditDataService;
            _logger = logger;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.ConfigureIdentity();
            builder.ConfigureGroup();
            builder.ConfigureUser();

            builder.ApplyConfiguration(new RoleAssignmentConfiguration());
            builder.ApplyConfiguration(new PermissionConfiguration());
            builder.ApplyConfiguration(new PermissionRoleConfiguration());

            builder.ApplyConfiguration(new InviteConfiguration());
            builder.ApplyConfiguration(new EmailConfiguration());


            builder.Entity<SessionEntity>().HasQueryFilter(x => x._DeletedDate == null);

            builder.ApplyConfiguration(new AuditEntityConfiguration());
        }

        public override int SaveChanges()
        {
            SoftDelete();
            AddAuditInfo();

            ProccessChangeTrackerResult proccessChangeTrackerResult = ChangeTrackerAuditService.ProccessChangeTracker(ChangeTracker);
            if(proccessChangeTrackerResult.RequiresCustomBatch)
            {
                using (IDbContextTransaction dbTransaction = Database.BeginTransaction())
                {
                    try
                    {
                        int changes = base.SaveChanges();

                        AuditSubjectData auditSubjectData = _auditDataService.Get();

                        IEnumerable<AuditEntity> auditEntities = proccessChangeTrackerResult.AuditObjectData
                            .Select(x => new AuditEntity(
                                auditObjectData: x,
                                auditSubjectData: auditSubjectData));

                        Audit.AddRange(auditEntities);

                        int auditChanges = base.SaveChanges();

                        dbTransaction.Commit();

                        return changes;
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to perform db transaction");

                        dbTransaction.Rollback();

                        throw;
                    }
                }
            }
            else
            {
                AuditSubjectData auditSubjectData = _auditDataService.Get();

                IEnumerable<AuditEntity> auditEntities = proccessChangeTrackerResult.AuditObjectData
                    .Select(x => new AuditEntity(
                        auditObjectData: x,
                        auditSubjectData: auditSubjectData));

                Audit.AddRange(auditEntities);

                return base.SaveChanges() - auditEntities.Count();
            }
        }

        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            SoftDelete();
            AddAuditInfo();

            ProccessChangeTrackerResult proccessChangeTrackerResult = ChangeTrackerAuditService.ProccessChangeTracker(ChangeTracker);
            if (proccessChangeTrackerResult.RequiresCustomBatch)
            {
                using (IDbContextTransaction dbTransaction = await Database.BeginTransactionAsync(cancellationToken))
                {
                    try
                    {
                        int changes = base.SaveChanges();

                        AuditSubjectData auditSubjectData = _auditDataService.Get();

                        IEnumerable<AuditEntity> auditEntities = proccessChangeTrackerResult.AuditObjectData
                            .Select(x => new AuditEntity(
                                auditObjectData: x,
                                auditSubjectData: auditSubjectData));

                        Audit.AddRange(auditEntities);

                        int auditChanges = await base.SaveChangesAsync(cancellationToken);

#if NET_CORE2
                        dbTransaction.Commit();
#elif NET_CORE3
                        await dbTransaction.CommitAsync(cancellationToken);
#endif
                        return changes;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to perform db transaction");

#if NET_CORE2
                        dbTransaction.Rollback();
#elif NET_CORE3
                        await dbTransaction.RollbackAsync(cancellationToken);
#endif

                        throw;
                    }
                }
            }
            else
            {
                AuditSubjectData auditSubjectData = _auditDataService.Get();

                IEnumerable<AuditEntity> auditEntities = proccessChangeTrackerResult.AuditObjectData
                    .Select(x => new AuditEntity(
                        auditObjectData: x,
                        auditSubjectData: auditSubjectData));

                Audit.AddRange(auditEntities);

                int changes = await base.SaveChangesAsync(cancellationToken);
                return changes - auditEntities.Count();
            }
        }

        private void AddAuditInfo()
        {
            IEnumerable<EntityEntry> entities = ChangeTracker
                .Entries()
                .Where(x => x.Entity is IBaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (EntityEntry entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((IBaseEntity)entity.Entity)._CreatedDate = DateTimeOffset.UtcNow;
                }
                ((IBaseEntity)entity.Entity)._ModifiedDate = DateTimeOffset.UtcNow;
            }
        }

        private void SoftDelete()
        {
            IEnumerable<EntityEntry> deletedEntites = ChangeTracker
                .Entries()
                .Where(x => x.Entity is ISoftDelete && (x.State == EntityState.Deleted));
            foreach (EntityEntry entity in deletedEntites)
            {
                entity.State = EntityState.Unchanged;
                ((ISoftDelete)entity.Entity)._DeletedDate = DateTime.UtcNow;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await SaveChangesAsync(new CancellationToken());
        }
    }
}
