using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Permission.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Interfaces.Services
{
    public interface IPermissionService
    {
        Result Add(AddPermissionRequest addPermission);
    }
}
