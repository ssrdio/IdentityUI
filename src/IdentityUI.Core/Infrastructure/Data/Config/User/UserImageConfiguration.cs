using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SSRD.IdentityUI.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Config.User
{
    internal class UserImageConfiguration : IEntityTypeConfiguration<UserImageEntity>
    {
        public void Configure(EntityTypeBuilder<UserImageEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FileName)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.BlobImage)
                .IsRequired();

            builder.HasOne(x => x.User)
                .WithOne(x => x.UserImage)
                .HasForeignKey<UserImageEntity>(x => x.UserId);

            builder.Ignore(x => x.URL);
        }
    }
}
