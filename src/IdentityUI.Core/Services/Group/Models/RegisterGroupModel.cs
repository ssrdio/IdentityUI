using FluentValidation;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Services.User.Models;
using SSRD.IdentityUI.Core.Services.User.Models.Add;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Group.Models
{
    public class RegisterGroupModel
    {
        public string GroupName { get; set; }

        public GroupBaseUserRegisterRequest BaseUser { get; set; }
    }

    public class RegisterGroupModelValidator : AbstractValidatorWithNullCheck<RegisterGroupModel>
    {
        public RegisterGroupModelValidator()
        {
            RuleFor(x => x.GroupName)
                .NotEmpty();

            RuleFor(x => x.BaseUser)
                .NotNull();
        }
    }
}
