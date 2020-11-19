using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Interfaces
{
    [Obsolete]
    public interface IUrlGenerator
    {
        [Obsolete("This function does not work properly if you use proxy or load balancer")]
        string GenerateActionUrl(string action, string controller, object values);
    }
}
