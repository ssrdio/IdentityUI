using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Data.Models.Constants;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSRD.IdentityUI.Core.Services.Group
{
    public class GroupStore : IGroupStore
    {
        private readonly IBaseRepository<GroupEntity> _groupRepository;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ILogger<GroupStore> _logger;

        public GroupStore(IBaseRepository<GroupEntity> groupRepository, IHttpContextAccessor httpContextAccessor, ILogger<GroupStore> logger)
        {
            _groupRepository = groupRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private TSpecification ApplayGroupFilter<TSpecification>(TSpecification specification)
            where TSpecification : BaseSpecification<GroupEntity>
        {
            if (_httpContextAccessor.HttpContext.User.HasPermission(IdentityUIPermissions.IDENTITY_UI_CAN_MANAGE_GROUPS))
            {
            }
            else if(_httpContextAccessor.HttpContext.User.GetGroupId() != null)
            {
                specification.AddFilter(x => x.Id == _httpContextAccessor.HttpContext.User.GetGroupId()); // TODO check for duplication
            }
            else
            {
                specification.AddFilter(x => false);
            }

            return specification;
        }

        public Result Exists(BaseSpecification<GroupEntity> baseSpecification)
        {
            baseSpecification = ApplayGroupFilter(baseSpecification);

            bool groupExist = _groupRepository.Exist(baseSpecification);
            if (!groupExist)
            {
                _logger.LogError($"No group.");
                return Result.Fail("no_group", "No Group");
            }

            return Result.Ok();
        }

        public Result Exists(string groupId)
        {
            BaseSpecification<GroupEntity> baseSpecification = new BaseSpecification<GroupEntity>();
            baseSpecification.AddFilter(x => x.Id == groupId);

            return Exists(baseSpecification);
        }

        public Result<GroupEntity> Get(string id)
        {
            BaseSpecification<GroupEntity> baseSpecification = new BaseSpecification<GroupEntity>();
            baseSpecification.AddFilter(x => x.Id == id);

            _logger.LogInformation($"Getting Group. GroupId {id}");

            return Get(baseSpecification);
        }

        public Result<GroupEntity> Get(BaseSpecification<GroupEntity> baseSpecification)
        {
            baseSpecification = ApplayGroupFilter(baseSpecification);

            GroupEntity group = _groupRepository.SingleOrDefault(baseSpecification);
            if (group == null)
            {
                _logger.LogError($"No group. GroupId");
                return Result.Fail<GroupEntity>("no_group", "No Group");
            }

            return Result.Ok(group);
        }

        public T Get<T>(SelectSpecification<GroupEntity, T> selectSpecification)
        {
            selectSpecification = ApplayGroupFilter(selectSpecification);

            return _groupRepository.SingleOrDefault(selectSpecification);
        }
    }
}
