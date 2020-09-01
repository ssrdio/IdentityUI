using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.User;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.User;
using SSRD.IdentityUI.Core.Data.Entities.User;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.User
{
    internal class UserAttributeDataService : IUserAttributeDataService
    {
        private readonly IBaseRepositoryAsync<UserAttributeEntity> _userAttributeRepository;

        private readonly IValidator<DataTableRequest> _dataTableValidator;

        private readonly ILogger<UserAttributeDataService> _logger;

        public UserAttributeDataService(IBaseRepositoryAsync<UserAttributeEntity> userAttributeRepository,
            IValidator<DataTableRequest> dataTableValidator,
            ILogger<UserAttributeDataService> logger)
        {
            _userAttributeRepository = userAttributeRepository;
            _dataTableValidator = dataTableValidator;
            _logger = logger;
        }

        public async Task<Result<DataTableResult<UserAttributeTableModel>>> Get(string userId, DataTableRequest dataTableRequest)
        {
            ValidationResult validationResult = _dataTableValidator.Validate(dataTableRequest);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {typeof(DataTableRequest).Name} model");
                return Result.Fail<DataTableResult<UserAttributeTableModel>>(validationResult.Errors);
            }

            PaginationSpecification<UserAttributeEntity, UserAttributeTableModel> paginationSpecification = new PaginationSpecification<UserAttributeEntity, UserAttributeTableModel>();
            paginationSpecification.AddFilter(x => x.UserId == userId);
            paginationSpecification.AddSelect(x => new UserAttributeTableModel(
                x.Id,
                x.Key,
                x.Value));
            paginationSpecification.AppalyPaging(dataTableRequest.Start, dataTableRequest.Length);

            PaginatedData<UserAttributeTableModel> paginatedData = await _userAttributeRepository.GetPaginated(paginationSpecification);

            DataTableResult<UserAttributeTableModel> dataTableResult = new DataTableResult<UserAttributeTableModel>(
                draw: dataTableRequest.Draw,
                recordsTotal: paginatedData.Count,
                recordsFilterd: paginatedData.Count,
                error: null,
                data: paginatedData.Data);

            return Result.Ok(dataTableResult);
        }
    }
}
