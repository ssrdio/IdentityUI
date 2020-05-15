using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Email.Models
{
    public class SendTesEmailRequest
    {
        public string Email { get; set; }
    }

    internal class SendTestEmailRequestValidator : AbstractValidator<SendTesEmailRequest>
    {
        public SendTestEmailRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
        }
    }
}
