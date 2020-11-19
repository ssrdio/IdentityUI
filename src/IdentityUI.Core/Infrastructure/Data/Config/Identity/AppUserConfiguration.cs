using SSRD.IdentityUI.Core.Data.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using SSRD.IdentityUI.Core.Services.Auth.TwoFactorAuth.Models;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Config.Identity
{
    internal class AppUserConfiguration : IEntityTypeConfiguration<AppUserEntity>
    {
        public void Configure(EntityTypeBuilder<AppUserEntity> builder)
        {
            builder.HasMany(e => e.Claims)
                .WithOne(e => e.User)
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();

            builder.HasMany(e => e.Logins)
                .WithOne(e => e.User)
                .HasForeignKey(ul => ul.UserId)
                .IsRequired();

            builder.HasMany(e => e.Tokens)
                .WithOne(e => e.User)
                .HasForeignKey(ut => ut.UserId)
                .IsRequired();

            builder.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            builder.HasMany(x => x.Sessions)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);

            builder.Property(x => x.FirstName)
                .HasMaxLength(256);

            builder.Property(x => x.LastName)
                .HasMaxLength(256);

            builder.Property(x => x.Enabled)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.TwoFactor)
                .IsRequired()
                .HasDefaultValue(TwoFactorAuthenticationType.None);

            builder.Ignore(x => x.SessionCode);

            builder.Ignore(x => x.ImpersonatorId);

            builder.ToTable("Users");
        }
    }
}
