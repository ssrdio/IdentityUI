using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SSRD.AdminUI.Template.Models;
using SSRD.Audit.Data;
using SSRD.CommonUtils.Result;
using SSRD.CommonUtils.Specifications;
using SSRD.CommonUtils.Specifications.Interfaces;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Interfaces;
using SSRD.IdentityUI.Admin.Areas.GroupAdmin.Models.Dashboard;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Dashboard;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.GroupAdmin.Services
{
    public class GroupAdminDashboardDataService : IGroupAdminDashboardService
    {
        private readonly IGroupUserStore _groupUserStore;
        private readonly IBaseDAO<AuditEntity> _auditDAO;

        private readonly IValidator<TimeRangeRequest> _timeRangeRequestValidator;

        private readonly ILogger<GroupAdminDashboardDataService> _logger;

        public GroupAdminDashboardDataService(
            IGroupUserStore groupUserStore,
            IBaseDAO<AuditEntity> auditDAO,
            IValidator<TimeRangeRequest> timeRangeRequestValidator,
            ILogger<GroupAdminDashboardDataService> logger)
        {
            _groupUserStore = groupUserStore;
            _auditDAO = auditDAO;

            _timeRangeRequestValidator = timeRangeRequestValidator;

            _logger = logger;
        }

        public async Task<Result<GroupedStatisticsViewModel>> GetIndexViewModel(string groupId)
        {
            IBaseSpecification<GroupUserEntity, GroupUserEntity> getAllUsersSpecification = SpecificationBuilder
                .Create<GroupUserEntity>()
                .Build();

            int allUsers = await _groupUserStore.Count(getAllUsersSpecification);

            IBaseSpecification<GroupUserEntity, GroupUserEntity> getActiveUsersSpecification = SpecificationBuilder
                .Create<GroupUserEntity>()
                .Where(x => x.User.Sessions.Any())
                .Build();

            int activeUsers = await _groupUserStore.Count(getActiveUsersSpecification);

            IBaseSpecification<GroupUserEntity, GroupUserEntity> getUnconfirmedUsersSpecification = SpecificationBuilder
                .Create<GroupUserEntity>()
                .Where(x => !x.User.EmailConfirmed)
                .Build();

            int unconfirmedUsers = await _groupUserStore.Count(getUnconfirmedUsersSpecification);

            IBaseSpecification<GroupUserEntity, GroupUserEntity> getDisabledUsersSpecification = SpecificationBuilder
                .Create<GroupUserEntity>()
                .Where(x => !x.User.Enabled)
                .Build();

            int disabledUsers = await _groupUserStore.Count(getDisabledUsersSpecification);

            GroupedStatisticsViewModel viewModel = new GroupedStatisticsViewModel(
                groupId: groupId,
                usersCount: allUsers,
                activeUsersCount: activeUsers,
                unconfirmedUsersCount: unconfirmedUsers,
                disabledUsersCount: disabledUsers);

            return Result.Ok(viewModel);
        }

        public async Task<Result<List<TimeRangeStatisticsModel>>> GetRegistrationStatistics(string groupId, TimeRangeRequest timeRangeRequest)
        {
            ValidationResult validationResult = _timeRangeRequestValidator.Validate(timeRangeRequest);
            if(!validationResult.IsValid)
            {
                _logger.LogError($"Invalid {typeof(TimeRangeRequest).Name} model");
                return Result.Fail<List<TimeRangeStatisticsModel>>(validationResult.ToResultError());
            }

            IBaseSpecification<GroupUserEntity, DateTimeOffset> specification = SpecificationBuilder
                .Create<GroupUserEntity>()
                .Where(x => x.User._CreatedDate != null)
                .Where(x => x.User._CreatedDate >= timeRangeRequest.From)
                .Where(x => x.User._CreatedDate < timeRangeRequest.To)
                .Where(x => x.GroupId == groupId)
                .Select(x => x.User._CreatedDate.Value)
                .Build();

            List<DateTimeOffset> dateTimes = await _groupUserStore.Get(specification);

            List<TimeRangeStatisticsModel> viewModel = dateTimes
                .GroupBy(x => new { x.UtcDateTime.Year, x.UtcDateTime.Month, x.UtcDateTime.Day })
                .Select(x => new TimeRangeStatisticsModel(
                    dateTime: new DateTime(x.Key.Year, x.Key.Month, x.Key.Day, 0, 0, 0, DateTimeKind.Utc),
                    value: x.Count()))
                .ToList();

            DateTime dateTime = new DateTime(
                timeRangeRequest.From.UtcDateTime.Year,
                timeRangeRequest.From.UtcDateTime.Month,
                timeRangeRequest.From.UtcDateTime.Day,
                0,
                0,
                0,
                DateTimeKind.Utc);

            for (; dateTime < timeRangeRequest.To; dateTime = dateTime.AddDays(1))
            {
                bool exists = viewModel
                    .Where(x => x.DateTime == dateTime)
                    .Any();

                if(!exists)
                {
                    viewModel.Add(new TimeRangeStatisticsModel(
                        dateTime: dateTime,
                        value: 0));
                }
            }

            viewModel = viewModel
                .OrderBy(x => x.DateTime)
                .ToList();

            return Result.Ok(viewModel);
        }

        public async Task<Result<List<TimeRangeStatisticsModel>>> GetActivityStatistics(string groupId, TimeRangeRequest timeRangeRequest)
        {
            ValidationResult validationResult = _timeRangeRequestValidator.Validate(timeRangeRequest);
            if (!validationResult.IsValid)
            {
                _logger.LogError($"Invalid {typeof(TimeRangeRequest).Name} model");
                return Result.Fail<List<TimeRangeStatisticsModel>>(validationResult.ToResultError());
            }

            IBaseSpecification<AuditEntity, DateTime> specification = SpecificationBuilder
                .Create<AuditEntity>()
                .Where(x => x.Created >= timeRangeRequest.From.UtcDateTime)
                .Where(x => x.Created < timeRangeRequest.To.UtcDateTime)
                .WithGroupIdentifier(groupId)
                .Select(x => x.Created)
                .Build();

            List<DateTime> dateTimes = await _auditDAO.Get(specification);

            List<TimeRangeStatisticsModel> viewModel = dateTimes
                .GroupBy(x => new { x.Year, x.Month, x.Day })
                .Select(x => new TimeRangeStatisticsModel(
                    dateTime: new DateTime(x.Key.Year, x.Key.Month, x.Key.Day, 0, 0, 0, DateTimeKind.Utc),
                    value: x.Count()))
                .ToList();

            DateTime dateTime = new DateTime(
                timeRangeRequest.From.UtcDateTime.Year,
                timeRangeRequest.From.UtcDateTime.Month,
                timeRangeRequest.From.UtcDateTime.Day,
                0,
                0,
                0,
                DateTimeKind.Utc);

            for (; dateTime < timeRangeRequest.To; dateTime = dateTime.AddDays(1))
            {
                bool exists = viewModel
                    .Where(x => x.DateTime == dateTime)
                    .Any();

                if (!exists)
                {
                    viewModel.Add(new TimeRangeStatisticsModel(
                        dateTime: dateTime,
                        value: 0));
                }
            }

            viewModel = viewModel
                .OrderBy(x => x.DateTime)
                .ToList();

            return Result.Ok(viewModel);
        }
    }
}
