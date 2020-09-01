using FluentValidation;
using SSRD.IdentityUI.Core.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.User.Models.Attribute
{
    public class UpdateUserAttributeModel
    {
        public string Value { get; set; }
    }

    public class UpdateUserAttributeModelValidator : AbstractValidatorWithNullCheck<UpdateUserAttributeModel>
    {
        public UpdateUserAttributeModelValidator()
        {
            RuleFor(x => x.Value);
        }
    }
}
