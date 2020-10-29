using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using System;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Interfaces
{
    public interface IGroupStore
    {
        [Obsolete("Use Any(string id)")]
        Models.Result.Result Exists(string groupId);
        [Obsolete("Use Any(IBaseSpecification<GroupEntity, GroupEntity> specification)")]
        Models.Result.Result Exists(Core.Data.Specifications.BaseSpecification<GroupEntity> baseSpecification);

        [Obsolete("Use SingleOrDefault(string id)")]
        Models.Result.Result<GroupEntity> Get(string id);
        [Obsolete("Use SingleOrDefault<TValue>(IBaseSpecification<GroupEntity, TValue> specification)")]
        Models.Result.Result<GroupEntity> Get(Core.Data.Specifications.BaseSpecification<GroupEntity> baseSpecification);
        [Obsolete("Use SingleOrDefault<TValue>(IBaseSpecification<GroupEntity, TValue> specification)")]
        T Get<T>(Core.Data.Specifications.SelectSpecification<GroupEntity, T> selectSpecification);

        Task<Result> Any(string id);
        Task<Result> Any(IBaseSpecification<GroupEntity, GroupEntity> specification);

        Task<Result<GroupEntity>> SingleOrDefault(string id);
        Task<Result<TValue>> SingleOrDefault<TValue>(IBaseSpecification<GroupEntity, TValue> specification);
    }
}
