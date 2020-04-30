using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SSRD.IdentityUI.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Config
{
    internal class InviteConfiguration : IEntityTypeConfiguration<InviteEntity>
    {
        public void Configure(EntityTypeBuilder<InviteEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder
                .Property(x => x.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder
                .Property(x => x.Email)
                .IsRequired();

            builder
                .Property(x => x.Token)
                .IsRequired();

            builder
                .HasOne(x => x.Group)
                .WithMany(x => x.Invites)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
