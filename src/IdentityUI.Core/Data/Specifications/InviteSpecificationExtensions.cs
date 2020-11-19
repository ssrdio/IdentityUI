using SSRD.CommonUtils.Specifications;
using SSRD.IdentityUI.Core.Data.Entities;

namespace SSRD.IdentityUI.Core.Data.Specifications
{
    public static class InviteSpecificationExtensions
    {
        public static IBaseSpecificationBuilder<InviteEntity> SearchByEmail(this IBaseSpecificationBuilder<InviteEntity> builder, string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return builder;
            }

            search = search.ToUpper();

            builder = builder.Where(x => x.Email.ToUpper().Contains(search));

            return builder;
        }
    }
}
