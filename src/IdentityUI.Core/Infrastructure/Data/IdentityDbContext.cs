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

namespace SSRD.IdentityUI.Core.Infrastructure.Data
{
    internal class IdentityDbContext : IdentityDbContext<AppUserEntity, RoleEntity, string, UserClaimEntity, UserRoleEntity, UserLoginEntity, RoleClaimEntity, UserTokenEntity>
    {
        public DbSet<SessionEntity> Sessions { get; set; }
        public DbSet<RoleAssignmentEntity> RoleAssignments { get; set; }
        public DbSet<PermissionEntity> Permissions { get; set; }
        public DbSet<PermissionRoleEntity> PermissionRole { get; set; }

        public DbSet<GroupAttributeEntity> GroupAttributes { get; set; }
        public DbSet<GroupEntity> Groups { get; set; }
        public DbSet<GroupUserEntity> GroupUsers { get; set; }

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
        {

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

            builder.ApplyConfiguration(new SessionConfiguration());
            builder.ApplyConfiguration(new RoleAssignmentConfiguration());
            builder.ApplyConfiguration(new PermissionConfiguration());
            builder.ApplyConfiguration(new PermissionRoleConfiguration());

            builder.Entity<SessionEntity>().HasQueryFilter(x => x._DeletedDate == null);
        }

        public override int SaveChanges()
        {
            SoftDelete();
            AddAuditInfo();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            SoftDelete();
            AddAuditInfo();
            return base.SaveChangesAsync(cancellationToken);
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
    }
}
