using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Config.Group
{
    internal class GroupUserConfiguration : IEntityTypeConfiguration<GroupUserEntity>
    {
        public void Configure(EntityTypeBuilder<GroupUserEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder
                .HasOne(x => x.User)
                .WithMany(x => x.Groups)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Property(x => x.UserId)
                .IsRequired();

            builder
                .HasOne(x => x.Group)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Property(x => x.GroupId)
                .IsRequired();

            builder
                .HasOne(x => x.Role)
                .WithMany()
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
