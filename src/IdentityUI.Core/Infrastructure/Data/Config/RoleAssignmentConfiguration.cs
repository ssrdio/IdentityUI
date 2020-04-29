using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SSRD.IdentityUI.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Config
{
    internal class RoleAssignmentConfiguration : IEntityTypeConfiguration<RoleAssignmentEntity>
    {
        public void Configure(EntityTypeBuilder<RoleAssignmentEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder
                .HasOne(x => x.Role)
                .WithMany(x => x.CanAssigne)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Property(x => x.RoleId)
                .IsRequired();

            builder
                .HasOne(x => x.CanAssigneRole)
                .WithMany(x => x.CanBeAssignedBy)
                .HasForeignKey(x => x.CanAssigneRoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Property(x => x.CanAssigneRoleId)
                .IsRequired();
        }
    }
}
