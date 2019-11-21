using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.User.Models
{
    public class UnlockUserRequest
    {
        public string UserId { get; set; }

        public UnlockUserRequest()
        {
        }

        public UnlockUserRequest(string userId)
        {
            UserId = userId;
        }
    }

    public class UnlockUserValidator : AbstractValidator<UnlockUserRequest>
    {
        public UnlockUserValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();
        }
    }
}
