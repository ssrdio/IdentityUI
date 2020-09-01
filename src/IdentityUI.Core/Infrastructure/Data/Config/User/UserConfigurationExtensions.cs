using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Config.User
{
    internal static class UserConfigurationExtensions
    {
        public static ModelBuilder ConfigureUser(this ModelBuilder builder)
        {
            builder.ApplyConfiguration(new SessionConfiguration());
            builder.ApplyConfiguration(new UserImageConfiguration());

            builder.ApplyConfiguration(new UserAttributeConfiuration());

            return builder;
        }
    }
}
