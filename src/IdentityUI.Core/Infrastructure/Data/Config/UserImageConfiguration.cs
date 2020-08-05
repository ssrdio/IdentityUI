using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SSRD.IdentityUI.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Config
{
    internal class UserImageConfiguration : IEntityTypeConfiguration<UserImageEntity>
    {
        public void Configure(EntityTypeBuilder<UserImageEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FileName)
                .HasMaxLength(250);

            builder.Ignore(x => x.FileName);
        }
    }
}
