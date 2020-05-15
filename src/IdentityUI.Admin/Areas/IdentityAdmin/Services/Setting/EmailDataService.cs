using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Interfaces.Setting;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.DataTable;
using SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Models.Setting.Email;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Helper;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Models.Result;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSRD.IdentityUI.Admin.Areas.IdentityAdmin.Services.Setting
{
    internal class EmailDataService : IEmailDataService
    {
        private readonly IBaseRepository<EmailEntity> _emailRepository;
        private readonly IBaseRepository<AppUserEntity> _userRepository;

        private readonly IValidator<DataTableRequest> _dataTableValidator;

        private readonly ILogger<EmailDataService> _logger;

        private readonly IdentityUIEndpoints _identityUIEndpoint;

        public EmailDataService(IBaseRepository<EmailEntity> emailRepository, IBaseRepository<AppUserEntity> userRepository,
            IValidator<DataTableRequest> dataTableValidator, ILogger<EmailDataService> logger, IOptionsSnapshot<IdentityUIEndpoints> identityUIEndpoints)
        {
            _emailRepository = emailRepository;
            _userRepository = userRepository;

            _dataTableValidator = dataTableValidator;

            _logger = logger;

            _identityUIEndpoint = identityUIEndpoints.Value;
        }

        public Result<DataTableResult<EmailTableModel>> Get(DataTableRequest dataTableRequest)
        {
            ValidationResult validationResult = _dataTableValidator.Validate(dataTableRequest);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(DataTableRequest)} model");
                return Result.Fail<DataTableResult<EmailTableModel>>(validationResult.Errors);
            }

            PaginationSpecification<EmailEntity, EmailTableModel> paginationSpecification = new PaginationSpecification<EmailEntity, EmailTableModel>();

            paginationSpecification.AddSelect(x => new EmailTableModel(
                x.Id,
                x.Type.GetDescription()));
            paginationSpecification.AppalyPaging(dataTableRequest.Start, dataTableRequest.Length);

            PaginatedData<EmailTableModel> paginatedData = _emailRepository.GetPaginated(paginationSpecification);

            DataTableResult<EmailTableModel> dataTableResult = new DataTableResult<EmailTableModel>(
                draw: dataTableRequest.Draw,
                recordsFilterd: paginatedData.Count,
                recordsTotal: paginatedData.Count,
                error: null,
                data: paginatedData.Data);

            return Result.Ok(dataTableResult);
        }

        public EmailIndexViewModel GetIndexViewModel()
        {
            EmailIndexViewModel emailIndex = new EmailIndexViewModel(
                useEmailSender: _identityUIEndpoint.UseEmailSender ?? false);

            return emailIndex;
        }

        public Result<EmailViewModel> GetViewModel(long id, string userId)
        {
            SelectSpecification<EmailEntity, EmailViewModel> selectSpecification = new SelectSpecification<EmailEntity, EmailViewModel>();
            selectSpecification.AddFilter(x => x.Id == id);
            selectSpecification.AddSelect(x => new EmailViewModel(
                x.Id,
                x.Type.GetDescription(),
                x.Subject,
                x.Body));

            EmailViewModel emailView = _emailRepository.SingleOrDefault(selectSpecification);
            if (emailView == null)
            {
                _logger.LogError($"No Email. EmailId id");
                return Result.Fail<EmailViewModel>("no_email", "No Email");
            }

            SelectSpecification<AppUserEntity, string> emailSpecification = new SelectSpecification<AppUserEntity, string>();
            emailSpecification.AddFilter(x => x.Id == userId);
            emailSpecification.AddSelect(X => X.Email);

            string email = _userRepository.SingleOrDefault(emailSpecification);

            emailView.Email = email;
            emailView.UseEmailSender = _identityUIEndpoint.UseEmailSender ?? false;

            return Result.Ok(emailView);
        }
    }
}
