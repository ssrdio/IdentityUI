using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Email.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services
{
    public class ManageEmailService : IManageEmailService
    {
        private readonly IBaseRepository<EmailEntity> _emailRepository;

        private readonly IEmailService _emailService;

        private readonly IValidator<EditEmailRequest> _editEmailValidator;
        private readonly IValidator<SendTesEmailRequest> _sendTestEmailValidator;

        private readonly ILogger<ManageEmailService> _logger;

        public ManageEmailService(IBaseRepository<EmailEntity> emailRepository, IEmailService emailService,
            IValidator<EditEmailRequest> editEmailValidator, IValidator<SendTesEmailRequest> sendTestEmailValidator, ILogger<ManageEmailService> logger)
        {
            _emailRepository = emailRepository;

            _emailService = emailService;

            _editEmailValidator = editEmailValidator;
            _sendTestEmailValidator = sendTestEmailValidator;

            _logger = logger;
        }

        private Result<EmailEntity> Get(long id)
        {
            BaseSpecification<EmailEntity> baseSpecification = new BaseSpecification<EmailEntity>();
            baseSpecification.AddFilter(x => x.Id == id);

            EmailEntity emailEntity = _emailRepository.SingleOrDefault(baseSpecification);
            if(emailEntity == null)
            {
                _logger.LogError($"No email. EmailId {id}");
                return Result.Fail<EmailEntity>("no_email", "No Email");
            }

            return Result.Ok(emailEntity);
        }

        public Result Edit(long id, EditEmailRequest editEmail)
        {
            _logger.LogInformation($"Editing Email. EmailId {id}");

            ValidationResult validationResult = _editEmailValidator.Validate(editEmail);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(EditEmailRequest)} model");
                return Result.Fail(validationResult.Errors);
            }

            Result<EmailEntity> getEmailResult = Get(id);
            if(getEmailResult.Failure)
            {
                return Result.Fail(getEmailResult.Errors);
            }

            EmailEntity emailEntity = getEmailResult.Value;

            emailEntity.Update(editEmail.Subject, editEmail.Body);
            bool updateResult = _emailRepository.Update(emailEntity);
            if(!updateResult)
            {
                _logger.LogError($"Failed to update email", "Failed to update email");
                return Result.Fail("failed_to_update_email", "Failed to update email");
            }

            return Result.Ok();
        }

        public async Task<Result> TestEmail(long id, SendTesEmailRequest sendTesEmail)
        {
            ValidationResult validationResult = _sendTestEmailValidator.Validate(sendTesEmail);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invalid {nameof(SendTesEmailRequest)} model");
                return Result.Fail(validationResult.Errors);
            }

            Result<EmailEntity> getEmailResult = Get(id);
            if(getEmailResult.Failure)
            {
                return Result.Fail(getEmailResult.Errors);
            }

            Result result = await _emailService.SendTest(sendTesEmail.Email, getEmailResult.Value);
            if(result.Failure)
            {
                return Result.Fail(result.Errors);
            }

            return Result.Ok();
        }
    }
}
