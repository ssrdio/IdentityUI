using SSRD.CommonUtils.Specifications;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.User
{
    public static class UserSpecificationExtensions
    {
        public static IBaseSpecificationBuilder<AppUserEntity> WithUsername(this IBaseSpecificationBuilder<AppUserEntity> builder, string username)
        {
            username = username.ToUpper();

            builder = builder.Where(x => x.NormalizedUserName == username);

            return builder;
        }

        public static IBaseSpecificationBuilder<AppUserEntity> WithEmail(this IBaseSpecificationBuilder<AppUserEntity> builder, string emil)
        {
            emil = emil.ToUpper();

            builder = builder.Where(x => x.NormalizedEmail == emil);

            return builder;
        }
        public static IBaseSpecificationBuilder<AppUserEntity> WithId(this IBaseSpecificationBuilder<AppUserEntity> builder, string id)
        {
            builder = builder.Where(x => x.Id == id);

            return builder;
        }
    }
}
