using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Enums.Entity;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Models.Result;
using Stubble.Core;
using Stubble.Core.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.Auth.Email
{
    internal class EmailService : IEmailService
    {
        private readonly IBaseRepository<EmailEntity> _mailRepository;

        private readonly IEmailSender _emailSender;

        private readonly ILogger<EmailService> _logger;

        private readonly StubbleVisitorRenderer _stubble;

        public EmailService(IBaseRepository<EmailEntity> mailRepository, IEmailSender emailSender, ILogger<EmailService> logger)
        {
            _mailRepository = mailRepository;
            _emailSender = emailSender;
            _logger = logger;

            _stubble = new StubbleBuilder().Build();
        }

        private Result<EmailEntity> GetMail(EmailTypes type)
        {
            BaseSpecification<EmailEntity> baseSpecification = new BaseSpecification<EmailEntity>();
            baseSpecification.AddFilter(x => x.Type == type);

            EmailEntity mail = _mailRepository.SingleOrDefault(baseSpecification);
            if(mail == null)
            {
                _logger.LogError($"No mail active mail with type {type}");
                return Result.Fail<EmailEntity>("no_mail", "No Mail");
            }

            return Result.Ok(mail);
        }

        private async Task<Result> Send(string email, string subject, string body)
        {
            try
            {
                await _emailSender.SendEmailAsync(email, subject, body);
            }
            catch(Exception)
            {
                return Result.Fail("failed_to_send_email", "Failed to send email");
            }

            return Result.Ok();
        }

        public Task<Result> SendTest(string email, EmailEntity emailEntity)
        {
            return Send(email, emailEntity.Subject, emailEntity.Body);
        }

        public Task<Result> SendInvite(string email, string token)
        {
            Result<EmailEntity> mailResult = GetMail(EmailTypes.Invite);
            if (mailResult.Failure)
            {
                return Task.FromResult(Result.Fail(mailResult.Errors));
            }

            EmailEntity mail = mailResult.Value;
            string body = _stubble.Render(mail.Body, new { token = token });

            return Send(email, mail.Subject, body);
        }

        public Task<Result> SendConfirmation(string email, string token)
        {
            Result<EmailEntity> mailResult = GetMail(EmailTypes.EmailConfirmation);
            if (mailResult.Failure)
            {
                return Task.FromResult(Result.Fail(mailResult.Errors));
            }

            EmailEntity mail = mailResult.Value;
            string body = _stubble.Render(mail.Body, new { token = token });

            return Send(email, mail.Subject, body);
        }

        public Task<Result> SendPasswordRecovery(string email, string token)
        {
            Result<EmailEntity> mailResult = GetMail(EmailTypes.PasswordRecovery);
            if (mailResult.Failure)
            {
                return Task.FromResult(Result.Fail(mailResult.Errors));
            }

            EmailEntity mail = mailResult.Value;
            string body = _stubble.Render(mail.Body, new { token = token });

            return Send(email, mail.Subject, body);
        }

        public Task<Result> SendPasswordWasReset(string email)
        {
            Result<EmailEntity> mailResult = GetMail(EmailTypes.PasswordWasReset);
            if (mailResult.Failure)
            {
                return Task.FromResult(Result.Fail(mailResult.Errors));
            }

            EmailEntity mail = mailResult.Value;

            return Send(email, mail.Subject, mail.Body);
        }

        public Task<Result> Send2faToken(string email, string token)
        {
            Result<EmailEntity> mailResult = GetMail(EmailTypes.TwoFactorAuthenticationToken);
            if (mailResult.Failure)
            {
                return Task.FromResult(Result.Fail(mailResult.Errors));
            }

            EmailEntity mail = mailResult.Value;
            string body = _stubble.Render(mail.Body, new { token = token });

            return Send(email, mail.Subject, body);
        }
    }
}
