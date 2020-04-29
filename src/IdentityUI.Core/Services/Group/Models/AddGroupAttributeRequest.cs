using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Group.Models
{
    public class AddGroupAttributeRequest
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    internal class AddGroupAttributeRequestValidator : AbstractValidator<AddGroupAttributeRequest>
    {
        public AddGroupAttributeRequestValidator()
        {
            RuleFor(x => x.Key)
                .NotEmpty();
        }
    }
}
