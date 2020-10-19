using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.Audit.Data
{
    public class AuditEntityConfiguration : IEntityTypeConfiguration<AuditEntity>
    {
        public void Configure(EntityTypeBuilder<AuditEntity> builder)
        {
            builder.HasKey(x => x.Id);

            //TODO: maybe add some indexes

            builder.Property(x => x.Created)
                .HasConversion(x => x, x => DateTime.SpecifyKind(x, DateTimeKind.Utc));
        }
    }
}
