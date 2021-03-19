using SSRD.CommonUtils.Validation;
using System.Collections.Generic;

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
