using FluentValidation;
using SSRD.CommonUtils.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.OpenIdConnect.Models
{
    public class UpdateClientIdModel
    {
        public string ClientId { get; set; }
    }

    public class UpdateClientIdModelValidator : AbstractValidatorWithNullCheck<UpdateClientIdModel>
    {
        public UpdateClientIdModelValidator()
        {
            RuleFor(x => x.ClientId)
                .NotEmpty();
        }
    }
}
