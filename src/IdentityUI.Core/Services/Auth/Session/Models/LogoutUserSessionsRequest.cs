using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.Auth.Session.Models
{
    public class LogoutUserSessionsRequest
    {
        public string UserId { get; set; }

        public LogoutUserSessionsRequest()
        {

        }

        public LogoutUserSessionsRequest(string userId)
        {
            UserId = userId;
        }
    }

    internal class LogoutUserSessionValidator : AbstractValidator<LogoutUserSessionsRequest>
    {
        public LogoutUserSessionValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();
        }
    }
}
