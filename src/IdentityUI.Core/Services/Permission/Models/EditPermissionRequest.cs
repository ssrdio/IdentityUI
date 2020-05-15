using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Permission.Models
{
    public class EditPermissionRequest
    {
        public string Description { get; set; }
    }

    internal class EditPermissionRequestValidator : AbstractValidator<EditPermissionRequest>
    {
        public EditPermissionRequestValidator()
        {
            RuleFor(x => x.Description);
        }
    }
}
