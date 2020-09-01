using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.User.Models.Attribute;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces.Services
{
    public interface IUserAttributeService
    {
        Task<Result> Add(string userId, AddUserAttributeModel addUserAttribute);
        Task<Result> Update(string userId, long attributeId, UpdateUserAttributeModel updateUserAttribute);
        Task<Result> Remove(string userId, long attributeId);
    }
}
