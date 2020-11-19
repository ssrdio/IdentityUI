using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces
{
    public interface IGroupUserStore
    {
        [Obsolete("Use Any(long id)")]
        Models.Result.Result Exists(long id);
        [Obsolete("Use Any(IBaseSpecification<GroupUserEntity, GroupUserEntity> specification)")]
        Models.Result.Result Exists(Core.Data.Specifications.BaseSpecification<GroupUserEntity> baseSpecification);

        [Obsolete("Use SingleOrDefault(long id)")]
        Models.Result.Result<GroupUserEntity> Get(long id);
        [Obsolete("Use SingleOrDefault(string userId, string groupId)")]
        Models.Result.Result<GroupUserEntity> Get(string userId, string groupId);
        [Obsolete("Use SingleOrDefault<TData>(IBaseSpecification<GroupUserEntity, TData> specification)")]
        Models.Result.Result<GroupUserEntity> Get(Core.Data.Specifications.BaseSpecification<GroupUserEntity> baseSpecification);
        [Obsolete("Use SingleOrDefault<TData>(IBaseSpecification<GroupUserEntity, TData> specification)")]
        T Get<T>(Core.Data.Specifications.SelectSpecification<GroupUserEntity, T> selectSpecification);

        [Obsolete("Use Get<TData>(IBaseSpecification<GroupUserEntity, TData> specification) and Count<TData>(IBaseSpecification<GroupUserEntity, TData> specification)")]
        PaginatedData<T> GetPaginated<T>(Core.Data.Specifications.PaginationSpecification<GroupUserEntity, T> paginationSpecification);

        [Obsolete("Use CanManageRoles()")]
        List<RoleListData> CanManageGroupRoles();
        [Obsolete("Use CanAssigneRoles()")]
        List<RoleListData> CanAssigneGroupRoles();

        bool CanChangeOwnRole();

        Task<Result> Any(long id);
        Task<Result> Any(IBaseSpecification<GroupUserEntity, GroupUserEntity> specification);

        Task<Result<GroupUserEntity>> SingleOrDefault(long id);
        Task<Result<GroupUserEntity>> SingleOrDefault(string userId, string groupId);
        Task<Result<TData>> SingleOrDefault<TData>(IBaseSpecification<GroupUserEntity, TData> specification);

        Task<List<TData>> Get<TData>(IBaseSpecification<GroupUserEntity, TData> specification);
        Task<int> Count<TData>(IBaseSpecification<GroupUserEntity, TData> specification);

        Task<List<RoleListData>> CanManageRoles();
        Task<List<RoleListData>> CanAssigneRoles();

        Task<Result> CanManageUser(long groupUserId);
        Task<Result> CanManageUser(GroupUserEntity groupUser);
    }
}
