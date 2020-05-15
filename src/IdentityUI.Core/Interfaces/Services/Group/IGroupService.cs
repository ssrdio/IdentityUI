using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Group.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Interfaces.Services.Group
{
    public interface IGroupService
    {
        Result Add(AddGroupRequest addGroup);
        Result Remove(string id);
    }
}
