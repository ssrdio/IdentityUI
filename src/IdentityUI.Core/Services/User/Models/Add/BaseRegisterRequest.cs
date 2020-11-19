using FluentValidation;
using SSRD.IdentityUI.Core.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.User.Models
{
    public class BaseRegisterRequest : IUserAttributeRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public string PhoneNumber { get; set; }
        public string Username { get; set; }

        public IDictionary<string, string> Attributes { get; set; }
    }

    public class BaseRegisterRequestValidator : AbstractValidatorWithNullCheck<BaseRegisterRequest>
    {
        public const string USE_EMAIL_AS_USERNAME = "use_email_as_username";
        public const string REQUIRE_EMAIL_USERNAME = "require_email_username";

        public const string REQUIRE_PASSWORD = "require_password";

        public BaseRegisterRequestValidator()
        {
            RuleFor(x => x.FirstName);

            RuleFor(x => x.LastName);

            RuleFor(x => x.PhoneNumber);

            RuleSet(USE_EMAIL_AS_USERNAME, () =>
            {
                RuleFor(x => x.Email)
                    .NotEmpty()
                    .EmailAddress();

                RuleFor(x => x.Username)
                    .Equal(x => x.Email)
                    .When(x => !string.IsNullOrEmpty(x.Username))
                    .WithErrorCode("username_must_be_the_same_as_email");
            });

            RuleSet(REQUIRE_EMAIL_USERNAME, () =>
            {
                RuleFor(x => x.Email)
                    .NotEmpty()
                    .EmailAddress();

                RuleFor(x => x.Username)
                    .NotEmpty();
            });

            RuleSet(REQUIRE_PASSWORD, () =>
            {
                RuleFor(x => x.Password)
                    .NotEmpty();

                RuleFor(x => x.ConfirmPassword)
                    .NotEmpty()
                    .WithErrorCode("confirm_password_must_be_the_same_as_password");
            });
        }
    }
}
