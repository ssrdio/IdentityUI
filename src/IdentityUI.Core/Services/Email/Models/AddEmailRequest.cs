using FluentValidation;
using SSRD.IdentityUI.Core.Data.Enums.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Email.Models
{
    public class AddEmailRequest
    {
        public string Name { get; set; }
        public EmailTypes? Type { get; set; }
    }

    internal class AddEmailRequestValidator : AbstractValidator<AddEmailRequest>
    {
        public AddEmailRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Type)
                .NotNull()
                .IsInEnum();
        }
    }
}
