﻿using FluentValidation;
using SSRD.CommonUtils.Validation;

namespace SSRD.IdentityUI.Core.Services.User.Models.Attribute
{
    public class AddUserAttributeModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class AddUserAttributeModelValidator : AbstractValidatorWithNullCheck<AddUserAttributeModel>
    {
        public AddUserAttributeModelValidator()
        {
            RuleFor(x => x.Key)
                .NotEmpty();

            RuleFor(x => x.Value);
        }
    }
}
