using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SSRD.IdentityUI.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Config
{
    internal class PermissionRoleConfiguration : IEntityTypeConfiguration<PermissionRoleEntity>
    {
        public void Configure(EntityTypeBuilder<PermissionRoleEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder
                .HasOne(x => x.Permission)
                .WithMany(x => x.Roles)
                .HasForeignKey(x => x.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Property(x => x.PermissionId)
                .IsRequired();

            builder
                .HasOne(x => x.Role)
                .WithMany(x => x.Permissions)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Property(X => X.RoleId)
                .IsRequired();
        }
    }
}
