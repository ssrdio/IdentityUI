using FluentValidation;
using SSRD.IdentityUI.Core.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Services.User.Models
{
    public interface IUserAttributeRequest
    {
        IDictionary<string, string> Attributes { get; set; }
    }

    public class NullUserAttributeRequestValidator : AbstractValidatorWithNullCheck<IUserAttributeRequest>
    {
        public NullUserAttributeRequestValidator()
        {
        }
    }
}
