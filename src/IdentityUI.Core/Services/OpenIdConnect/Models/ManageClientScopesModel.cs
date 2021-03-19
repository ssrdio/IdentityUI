using FluentValidation;
using SSRD.CommonUtils.Validation;
using System.Collections.Generic;

namespace SSRD.IdentityUI.Core.Services.OpenIdConnect.Models
{
    public class ManageClientScopesModel
    {
        public List<string> Scopes { get; set; }
    }

    public class ManageClientScopesModelValidator : AbstractValidatorWithNullCheck<ManageClientScopesModel>
    {
        public ManageClientScopesModelValidator()
        {
            RuleFor(x => x.Scopes)
                .NotNull();
        }
    }
}
