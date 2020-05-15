using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Core.Interfaces
{
    public interface IGroupUserStore
    {
        Result Exists(long id);
        Result Exists(BaseSpecification<GroupUserEntity> baseSpecification);

        Result<GroupUserEntity> Get(long id);
        Result<GroupUserEntity> Get(string userId, string groupId);
        Result<GroupUserEntity> Get(BaseSpecification<GroupUserEntity> baseSpecification);
        T Get<T>(SelectSpecification<GroupUserEntity, T> selectSpecification);

        PaginatedData<T> GetPaginated<T>(PaginationSpecification<GroupUserEntity, T> paginationSpecification);

        List<RoleListData> CanManageGroupRoles();
        List<RoleListData> CanAssigneGroupRoles();

        bool CanChangeOwnRole();
    }
}
