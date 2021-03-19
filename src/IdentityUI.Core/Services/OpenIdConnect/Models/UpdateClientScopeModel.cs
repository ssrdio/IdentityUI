using FluentValidation;
using SSRD.CommonUtils.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.OpenIdConnect.Models
{
    public class UpdateClientScopeModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }

    public class UpdateClientScopeModelValidator : AbstractValidatorWithNullCheck<UpdateClientScopeModel>
    {
        public UpdateClientScopeModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.DisplayName);

            RuleFor(x => x.Description);
        }
    }
}
