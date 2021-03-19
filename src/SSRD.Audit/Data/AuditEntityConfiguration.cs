using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace SSRD.Audit.Data
{
    public static class AuditConfiguration
    {
        public static void ConfigureAudit(this ModelBuilder builder)
        {
            builder.ApplyConfiguration(new AuditEntityConfiguration());
            builder.ApplyConfiguration(new AuditCommentEntityConfiguration());
        }
    }

    public class AuditEntityConfiguration : IEntityTypeConfiguration<AuditEntity>
    {
        public void Configure(EntityTypeBuilder<AuditEntity> builder)
        {
            builder.HasKey(x => x.Id);

            //TODO: maybe add some indexes

            builder
                .Property(x => x.Created)
                .HasConversion(x => x, x => DateTime.SpecifyKind(x, DateTimeKind.Utc));
        }
    }

    public class AuditCommentEntityConfiguration : IEntityTypeConfiguration<AuditCommentEntity>
    {
        public void Configure(EntityTypeBuilder<AuditCommentEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder
                .Property(x => x.UserId)
                .IsRequired();

            builder
                .Property(x => x.Comment)
                .IsRequired();

            builder
                .HasOne(x => x.Audit)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.AuditId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Property(x => x.Created)
                .HasConversion(x => x, x => DateTime.SpecifyKind(x, DateTimeKind.Utc));
        }
    }
}
