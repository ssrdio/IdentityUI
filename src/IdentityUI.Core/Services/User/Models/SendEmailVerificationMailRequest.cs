using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.User.Models
{
    public class SendEmailVerificationMailRequest
    {
        public string UserId { get; set; }

        public SendEmailVerificationMailRequest()
        {
        }

        public SendEmailVerificationMailRequest(string userId)
        {
            UserId = userId;
        }
    }

    public class SendEmailVerificationMailValidtor : AbstractValidator<SendEmailVerificationMailRequest>
    {
        public SendEmailVerificationMailValidtor()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();
        }
    }
}
