using SSRD.CommonUtils.Specifications;
using SSRD.IdentityUI.Core.Data.Entities.Identity;

namespace SSRD.IdentityUI.Core.Data.Specifications
{
    public static class AppUserSpecificationExtensions
    {
        public static IBaseSpecificationBuilder<AppUserEntity> SearchByUsername(this IBaseSpecificationBuilder<AppUserEntity> builder, string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return builder;
            }

            search = search.ToUpper();

            builder = builder.Where(x => x.NormalizedUserName.Contains(search));

            return builder;
        }
    }
}
