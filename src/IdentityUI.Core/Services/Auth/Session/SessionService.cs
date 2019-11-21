using FluentValidation;
using FluentValidation.Results;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Enums.Entity;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Models.Result;
using SSRD.IdentityUI.Core.Services.Auth.Session.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSRD.IdentityUI.Core.Services.Auth.Session
{
    internal class SessionService : ISessionService
    {
        private readonly UserManager<AppUserEntity> _userManager;

        private readonly ISessionRepository _sessionRepository;

        private readonly IValidator<LogoutSessionRequest> _logoutSessionValidator;
        private readonly IValidator<LogoutUserSessionsRequest> _logoutUserSessionsValidator;

        private readonly ILogger<SessionService> _logger;

        public SessionService(UserManager<AppUserEntity> userManager, ISessionRepository sessionRepository,
            IValidator<LogoutSessionRequest> logoutSessionValidator, IValidator<LogoutUserSessionsRequest> logoutUserSessionValidator,
            ILogger<SessionService> logger)
        {
            _userManager = userManager;

            _sessionRepository = sessionRepository;

            _logoutSessionValidator = logoutSessionValidator;
            _logoutUserSessionsValidator = logoutUserSessionValidator;

            _logger = logger;
        }

        public Result Logout(LogoutSessionRequest request, string adminId)
        {
            ValidationResult validationResult = _logoutSessionValidator.Validate(request);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invlid LogoutSessionRequest. Admin {adminId}");
                return Result.Fail(validationResult.Errors);
            }

            BaseSpecification<SessionEntity> specification = new BaseSpecification<SessionEntity>();
            specification.AddFilter(x => x.Id == request.SessionId);
            specification.AddFilter(x => x.UserId == request.UserId);

            SessionEntity session = _sessionRepository.Get(specification);
            if (session == null)
            {
                _logger.LogError($"Sesion does not exist. SessionId {request.SessionId}, Admin {adminId}");
                return Result.Fail("no_session", "No Session");
            }

            session.EndType = SessionEndTypes.AdminLogout;

            bool removeSessionResult = _sessionRepository.Remove(session);
            if (!removeSessionResult)
            {
                _logger.LogError($"Faild to remove session. SessionId {request.SessionId}, Admin {adminId}");
                return Result.Fail("error", "Error");
            }

            _logger.LogInformation($"Sessions logout. UserId {request.SessionId}, Admin {adminId}");

            return Result.Ok();
        }

        public async Task<Result> LogoutUser(LogoutUserSessionsRequest request, string adminId)
        {
            ValidationResult validationResult = _logoutUserSessionsValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invlid LogoutSessionRequest. Admin {adminId}");
                return Result.Fail(validationResult.Errors);
            }

            AppUserEntity appUser = await _userManager.FindByIdAsync(request.UserId);
            if (appUser == null)
            {
                _logger.LogWarning($"No user. UserId {request.UserId}, Admin {adminId}");
                return Result.Fail("no_user", "No user");
            }

            SelectSpecification<SessionEntity, SessionEntity> specification = new SelectSpecification<SessionEntity, SessionEntity>();
            specification.AddFilter(x => x.UserId == request.UserId);
            specification.AddSelect(x => x);

            List<SessionEntity> sessions = _sessionRepository.GetList(specification);
            if(sessions.Count == 0)
            {
                return Result.Ok();
            }

            foreach(SessionEntity session in sessions)
            {
                session.EndType = SessionEndTypes.AdminLogout;
            }

            bool removeResult = await _sessionRepository.Remove(sessions);
            if (!removeResult)
            {
                _logger.LogWarning($"Faild to remove sessions. UserId {request.UserId}, Admin {adminId}");
                return Result.Fail("error", "Error");
            }

            _logger.LogInformation($"User session logout. Admin {adminId}");

            return Result.Ok();
        }


        public Result Logout(string sessionCode, string userId, SessionEndTypes endType)
        {
            BaseSpecification<SessionEntity> specification = new BaseSpecification<SessionEntity>();
            specification.AddFilter(x => x.Code == sessionCode);
            specification.AddFilter(x => x.UserId == userId);

            SessionEntity session = _sessionRepository.Get(specification);
            if (session == null)
            {
                _logger.LogError($"Sesion does not exist. SessionCode {sessionCode}, userId {userId}");
                return Result.Fail("no_session", "No Session");
            }

            session.EndType = endType;

            bool removeSessionResult = _sessionRepository.Remove(session);
            if (!removeSessionResult)
            {
                _logger.LogError($"Faild to remove session. SessionCode {sessionCode}, userId {userId}");
                return Result.Fail("error", "Error");
            }

            _logger.LogInformation($"User session logout. UserId {userId}");

            return Result.Ok();
        }

        public bool Validate(string sessionCode, string userId, string ip)
        {
            BaseSpecification<SessionEntity> specification = new BaseSpecification<SessionEntity>();
            specification.AddFilter(x => x.Code == sessionCode);
            specification.AddFilter(x => x.UserId == userId);

            SessionEntity session = _sessionRepository.Get(specification);
            if(session == null)
            {
                return false;
            }

            session.Ip = ip;
            session.LastAccess = DateTimeOffset.UtcNow;

            return _sessionRepository.Update(session);
        }

        public Result Add(string code, string userId, string ip)
        {
            SessionEntity session = new SessionEntity(
                ip: ip,
                userId: userId,
                code: code);

            bool addResult = _sessionRepository.Add(session);
            if(!addResult)
            {
                _logger.LogError($"Faild to add session. UserId {userId}");
                return Result.Fail("error", "Error");
            }

            return Result.Ok();
        }
    }
}
