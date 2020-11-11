using SSRD.CommonUtils.Specifications;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Data.Specifications
{
    public static class GroupUserSpecificationExtensions
    {
        public static IBaseSpecificationBuilder<GroupUserEntity> SearchByUsername(this IBaseSpecificationBuilder<GroupUserEntity> builder, string search)
        {
            if(string.IsNullOrEmpty(search))
            {
                return builder;
            }

            search = search.ToUpper();

            builder = builder.Where(x => x.User.NormalizedUserName.Contains(search));

            return builder;
        }

        public static IBaseSpecificationBuilder<GroupUserEntity> SearchByUsernameEmailId(this IBaseSpecificationBuilder<GroupUserEntity> builder, string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return builder;
            }

            search = search.ToUpper();

            builder = builder.Where(x => 
                x.User.NormalizedUserName.Contains(search)
                || x.User.NormalizedEmail.Contains(search)
                || x.User.Id.ToUpper().Contains(search));

            return builder;
        }
    }
}
