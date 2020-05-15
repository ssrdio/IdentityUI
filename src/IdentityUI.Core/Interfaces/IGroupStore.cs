using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Interfaces
{
    public interface IGroupStore
    {
        Result Exists(string groupId);
        Result Exists(BaseSpecification<GroupEntity> baseSpecification);

        Result<GroupEntity> Get(string id);
        Result<GroupEntity> Get(BaseSpecification<GroupEntity> baseSpecification);
        T Get<T>(SelectSpecification<GroupEntity, T> selectSpecification);
    }
}
