using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.Group;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Group;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Models;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.Group
{
    internal class GroupInviteDataService : IGroupInviteDataService
    {
        private readonly IBaseRepository<InviteEntity> _inviteRepository;
        private readonly IValidator<DataTableRequest> _dataTableRequestValidator;
        private readonly ILogger<GroupInviteDataService> _logger;

        public GroupInviteDataService(IBaseRepository<InviteEntity> inviteRepository, IValidator<DataTableRequest> dataTableRequestValidator,
            ILogger<GroupInviteDataService> logger)
        {
            _inviteRepository = inviteRepository;
            _dataTableRequestValidator = dataTableRequestValidator;
            _logger = logger;
        }

        public Result<DataTableResult<GroupInviteTableModel>> Get(string groupId, DataTableRequest dataTableRequest)
        {
            ValidationResult validationResult = _dataTableRequestValidator.Validate(dataTableRequest);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(DataTableRequest)} model");
                return Result.Fail<DataTableResult<GroupInviteTableModel>>(validationResult.Errors);
            }

            PaginationSpecification<InviteEntity, GroupInviteTableModel> paginationSpecification =
                new PaginationSpecification<InviteEntity, GroupInviteTableModel>();

            paginationSpecification.AddFilter(x => x.GroupId == groupId);
            if(!string.IsNullOrEmpty(dataTableRequest.Search))
            {
                paginationSpecification.AddFilter(x => x.Email.ToUpper().Contains(dataTableRequest.Search.ToUpper()));
            }

            paginationSpecification.AddSelect(x => new GroupInviteTableModel(
                x.Id,
                x.Email,
                x.GroupRole.Name,
                x.Status.ToString(),
                x.ExpiresAt.ToString(DateTimeFormats.DEFAULT_DATE_TIME_FORMAT)));

            paginationSpecification.AppalyPaging(dataTableRequest.Start, dataTableRequest.Length);

            PaginatedData<GroupInviteTableModel> paginatedData = _inviteRepository.GetPaginated(paginationSpecification);

            DataTableResult<GroupInviteTableModel> dataTableResult = new DataTableResult<GroupInviteTableModel>(
                draw: dataTableRequest.Draw,
                recordsTotal: paginatedData.Count,
                recordsFilterd: paginatedData.Count,
                error: null,
                data: paginatedData.Data);

            return Result.Ok(dataTableResult);
        }
    }
}
