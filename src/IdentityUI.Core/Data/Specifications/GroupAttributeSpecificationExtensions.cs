using SSRD.CommonUtils.Specifications;
using SSRD.IdentityUI.Core.Data.Entities.Group;

namespace SSRD.IdentityUI.Core.Data.Specifications
{
    public static class GroupAttributeSpecificationExtensions
    {
        public static IBaseSpecificationBuilder<GroupAttributeEntity> SearchByKey(
            this IBaseSpecificationBuilder<GroupAttributeEntity> builder,
            string search)
        {
            if(string.IsNullOrEmpty(search))
            {
                return builder;
            }

            search = search.ToUpper();

            builder = builder.Where(x => x.Key.Contains(search));

            return builder;
        }
    }
}
