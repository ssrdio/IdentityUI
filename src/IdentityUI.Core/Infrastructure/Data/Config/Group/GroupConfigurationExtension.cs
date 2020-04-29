using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Infrastructure.Data.Config.Group
{
    internal static class GroupConfigurationExtension
    {
        public static void ConfigureGroup(this ModelBuilder builder)
        {
            builder.ApplyConfiguration(new GroupAttributeConfiguration());
            builder.ApplyConfiguration(new GroupConfiguration());
            builder.ApplyConfiguration(new RoleAssignmentConfiguration());
            builder.ApplyConfiguration(new GroupUserConfiguration());
        }
    }
}
