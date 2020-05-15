using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Config.Group
{
    internal class GroupAttributeConfiguration : IEntityTypeConfiguration<GroupAttributeEntity>
    {
        public void Configure(EntityTypeBuilder<GroupAttributeEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder
                .Property(x => x.Key)
                .IsRequired();

            builder
                .HasOne(x => x.Group)
                .WithMany(x => x.GroupAttributes)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Property(x => x.GroupId)
                .IsRequired();

            builder.HasIndex(x => x.Key); //TODO: maybe add GroupId + Key index
        }
    }
}
