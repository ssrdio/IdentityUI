using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SSRD.IdentityUI.Core.Data.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Config.User
{
    internal class UserAttributeConfiuration : IEntityTypeConfiguration<UserAttributeEntity>
    {
        public void Configure(EntityTypeBuilder<UserAttributeEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Key)
                .IsRequired();

            builder.HasIndex(x => x.Key);

            builder.Property(x => x.Value);

            builder
                .HasOne(x => x.User)
                .WithMany(x => x.Attributes)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
