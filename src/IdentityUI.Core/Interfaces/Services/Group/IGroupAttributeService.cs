using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Group.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Interfaces.Services.Group
{
    public interface IGroupAttributeService
    {
        Result Add(string groupId, AddGroupAttributeRequest addGroupAttribute);
        Result Edit(string groupId, long id, EditGroupAttributeRequest editGroupAttribute);
        Result Remove(string groupId, long id);
    }
}
