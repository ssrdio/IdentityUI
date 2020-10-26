using SSRD.CommonUtils.Specifications;
using SSRD.IdentityUI.Core.Data.Entities.Group;

namespace SSRD.IdentityUI.Core.Services.Group
{
    public static class GroupSpecificationExtensions
    {
        public static IBaseSpecificationBuilder<GroupEntity> WithName(this IBaseSpecificationBuilder<GroupEntity> builder, string name)
        {
            name = name.ToUpper();

            builder = builder.Where(x => x.Name.ToUpper() == name);

            return builder;
        }

        public static IBaseSpecificationBuilder<GroupEntity> WithId(this IBaseSpecificationBuilder<GroupEntity> builder, string id)
        {
            builder = builder.Where(x => x.Id == id);

            return builder;
        }
    }
}
