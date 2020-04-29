using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Permission.Models
{
    public class AddPermissionRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    internal class AddPermissionRequestValidator : AbstractValidator<AddPermissionRequest>
    {
        public AddPermissionRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Description);
        }
    }
}
