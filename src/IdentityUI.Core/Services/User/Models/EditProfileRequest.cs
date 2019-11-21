using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.User.Models
{
    public class EditProfileRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
    }

    internal class EditProfileValidator : AbstractValidator<EditProfileRequest>
    {
        public EditProfileValidator()
        {
            RuleFor(x => x.FirstName);

            RuleFor(x => x.LastName);

            RuleFor(x => x.PhoneNumber);
        }
    }
}
