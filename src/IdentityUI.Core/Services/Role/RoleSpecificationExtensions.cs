using SSRD.CommonUtils.Specifications;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Role
{
    public static class RoleSpecificationExtensions
    {
        public static IBaseSpecificationBuilder<RoleEntity> WithName(this IBaseSpecificationBuilder<RoleEntity> builder, string name)
        {
            if(name == null)
            {
                throw new ArgumentNullException("name_cannot_be_null");
            }

            name = name.ToUpper();

            builder = builder.Where(x => x.NormalizedName == name);

            return builder;
        }
    }
}
