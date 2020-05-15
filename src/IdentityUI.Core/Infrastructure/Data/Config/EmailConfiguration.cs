using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SSRD.IdentityUI.Core.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Config
{
    internal class EmailConfiguration : IEntityTypeConfiguration<EmailEntity>
    {
        public void Configure(EntityTypeBuilder<EmailEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder
                .Property(x => x.Subject)
                .IsRequired();

            builder
                .Property(x => x.Body)
                .IsRequired();
        }
    }
}
