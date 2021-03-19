using FluentValidation;
using SSRD.CommonUtils.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.OpenIdConnect.Models
{
    public class AddClientScopeModel
    {
        public string Name { get; set; }
    }

    public class AddClientScopeModelValidator : AbstractValidatorWithNullCheck<AddClientScopeModel>
    {
        public AddClientScopeModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
