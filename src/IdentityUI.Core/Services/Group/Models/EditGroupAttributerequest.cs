using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Group.Models
{
    public class EditGroupAttributeRequest
    {
        public string Value { get; set; }
    }

    internal class EditGroupAttributeRequestValidator : AbstractValidator<EditGroupAttributeRequest>
    {
        public EditGroupAttributeRequestValidator()
        {
            RuleFor(x => x.Value);
        }
    }
}
