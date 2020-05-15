using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.Group;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Group;
using SSRD.IdentityUI.Core.Data.Entities.Group;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.Group
{
    internal class GroupAttributeDataService : IGroupAttributeDataService
    {
        private readonly IBaseRepository<GroupAttributeEntity> _groupAttributeRepository;
        private readonly IValidator<DataTableRequest> _dataTableValidator;
        private readonly ILogger<GroupAttributeDataService> _logger;

        public GroupAttributeDataService(IBaseRepository<GroupAttributeEntity> groupAttributeRepository,
            IValidator<DataTableRequest> dataTableValidator, ILogger<GroupAttributeDataService> logger)
        {
            _groupAttributeRepository = groupAttributeRepository;
            _dataTableValidator = dataTableValidator;
            _logger = logger;
        }

        public Result<DataTableResult<GroupAttributeTableModel>> Get(string groupId, DataTableRequest dataTableRequest)
        {
            ValidationResult validationResult = _dataTableValidator.Validate(dataTableRequest);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(dataTableRequest)} model");
                return Result.Fail<DataTableResult<GroupAttributeTableModel>>(validationResult.Errors);
            }

            PaginationSpecification<GroupAttributeEntity, GroupAttributeTableModel> paginationSpecification =
                new PaginationSpecification<GroupAttributeEntity, GroupAttributeTableModel>();

            paginationSpecification.AddFilter(x => x.GroupId == groupId);
            if(!string.IsNullOrEmpty(dataTableRequest.Search))
            {
                paginationSpecification.AddFilter(x => x.Key.ToUpper().Contains(dataTableRequest.Search.ToUpper()));
            }

            paginationSpecification.AddSelect(x => new GroupAttributeTableModel(
                x.Id,
                x.Key,
                x.Value));
            paginationSpecification.AppalyPaging(dataTableRequest.Start, dataTableRequest.Length);

            PaginatedData<GroupAttributeTableModel> paginatedData = _groupAttributeRepository.GetPaginated(paginationSpecification);

            DataTableResult<GroupAttributeTableModel> dataTableResult = new DataTableResult<GroupAttributeTableModel>(
                draw: dataTableRequest.Draw,
                recordsTotal: paginatedData.Count,
                recordsFilterd: paginatedData.Count,
                error: null,
                data: paginatedData.Data);

            return Result.Ok(dataTableResult);
        }
    }
}
