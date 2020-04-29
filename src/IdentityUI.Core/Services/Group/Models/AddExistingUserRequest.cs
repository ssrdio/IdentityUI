using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Group.Models
{
    public class AddExistingUserRequest
    {
        public string UserId { get; set; }
    }

    internal class AddExisingUserRequestValidator : AbstractValidator<AddExistingUserRequest>
    { 
        public AddExisingUserRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();
        }
    }
}
