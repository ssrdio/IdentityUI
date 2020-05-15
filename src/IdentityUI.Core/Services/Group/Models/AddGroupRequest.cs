using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Group.Models
{
    public class AddGroupRequest
    {
        public string Name { get; set; }
    }

    internal class AddGroupRequestValidator : AbstractValidator<AddGroupRequest>
    {
        public AddGroupRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
