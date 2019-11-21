using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Role.Models
{
    public class NewRoleRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    internal class NewRoleValidator : AbstractValidator<NewRoleRequest>
    {
        public NewRoleValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Description)
                .MaximumLength(256);
        }
    }
}
