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
        public string DisplayName { get; set; }
        public string Description { get; set; }

        public AddClientScopeModel()
        {
        }

        public AddClientScopeModel(string name, string displayName, string description)
        {
            Name = name;
            DisplayName = displayName;
            Description = description;
        }
    }

    public class AddClientScopeModelValidator : AbstractValidatorWithNullCheck<AddClientScopeModel>
    {
        public AddClientScopeModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.DisplayName);

            RuleFor(x => x.Description);
        }
    }
}
