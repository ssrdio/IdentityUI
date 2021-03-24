using FluentValidation;
using SSRD.CommonUtils.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.OpenIdConnect.Models
{
    public class AddClientCredentialsClientModel
    {
        public string Name { get; set; }
        public string ClientId { get; set; }
    }

    public class AddClientCredentialsClientModelValidator : AbstractValidatorWithNullCheck<AddClientCredentialsClientModel>
    {
        public AddClientCredentialsClientModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.ClientId)
                .NotEmpty();
        }
    }
}
