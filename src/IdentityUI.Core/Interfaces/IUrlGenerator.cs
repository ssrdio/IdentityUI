using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Interfaces
{
    public interface IUrlGenerator
    {
        string GenerateActionUrl(string action, string controller, object values);
    }
}
