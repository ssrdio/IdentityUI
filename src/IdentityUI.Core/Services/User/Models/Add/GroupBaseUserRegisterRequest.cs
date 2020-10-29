using SSRD.IdentityUI.Core.Helper;

namespace SSRD.IdentityUI.Core.Services.User.Models.Add
{
    public class GroupBaseUserRegisterRequest : BaseRegisterRequest
    {
    }

    public class GroupBaseUserRegisterRequestValidator : AbstractValidatorWithNullCheck<GroupBaseUserRegisterRequest>
    {
        public GroupBaseUserRegisterRequestValidator()
        {

        }
    }
}
