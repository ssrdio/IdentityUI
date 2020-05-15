using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Email.Models
{
    public class EditEmailRequest
    {
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    internal class EditEmailRequestValidator : AbstractValidator<EditEmailRequest>
    {
        public EditEmailRequestValidator()
        {
            RuleFor(x => x.Subject)
                .NotNull();

            RuleFor(x => x.Body)
                .NotNull();
        }
    }
}
