using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Auth.Session.Models
{
    public class LogoutSessionRequest
    {
        public long SessionId { get; set; }
        public string UserId { get; set; }
    }

    internal class LogoutSessionValidator : AbstractValidator<LogoutSessionRequest>
    {
        public LogoutSessionValidator()
        {
            RuleFor(x => x.SessionId)
                .GreaterThan(0);

            RuleFor(x => x.UserId)
                .NotEmpty();
        }
    }
}
