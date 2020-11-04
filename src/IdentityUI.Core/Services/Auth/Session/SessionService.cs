using FluentValidation;
using FluentValidation.Results;
using SSRD.IdentityUI.Core.Data.Entities;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Enums.Entity;
using SSRD.IdentityUI.Core.Data.Specifications;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Services.Auth.Session.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SSRD.CommonUtils.Result;
using SSRD.IdentityUI.Core.Helper;

namespace SSRD.IdentityUI.Core.Services.Auth.Session
{
    internal class SessionService : ISessionService
    {
        private readonly UserManager<AppUserEntity> _userManager;

        private readonly ISessionRepository _sessionRepository;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IValidator<LogoutSessionRequest> _logoutSessionValidator;
        private readonly IValidator<LogoutUserSessionsRequest> _logoutUserSessionsValidator;

        private readonly ILogger<SessionService> _logger;

        public SessionService(
            UserManager<AppUserEntity> userManager,
            ISessionRepository sessionRepository,
            IHttpContextAccessor httpContextAccessor,
            IValidator<LogoutSessionRequest> logoutSessionValidator,
            IValidator<LogoutUserSessionsRequest> logoutUserSessionValidator,
            ILogger<SessionService> logger)
        {
            _userManager = userManager;

            _sessionRepository = sessionRepository;
            _httpContextAccessor = httpContextAccessor;

            _logoutSessionValidator = logoutSessionValidator;
            _logoutUserSessionsValidator = logoutUserSessionValidator;

            _logger = logger;
        }

        public Core.Models.Result.Result Logout(LogoutSessionRequest request, string adminId)
        {
            ValidationResult validationResult = _logoutSessionValidator.Validate(request);
            if(!validationResult.IsValid)
            {
                _logger.LogWarning($"Invlid LogoutSessionRequest. Admin {adminId}");
                return Core.Models.Result.Result.Fail(validationResult.Errors);
            }

            BaseSpecification<SessionEntity> specification = new BaseSpecification<SessionEntity>();
            specification.AddFilter(x => x.Id == request.SessionId);
            specification.AddFilter(x => x.UserId == request.UserId);

            SessionEntity session = _sessionRepository.SingleOrDefault(specification);
            if (session == null)
            {
                _logger.LogError($"Sesion does not exist. SessionId {request.SessionId}, Admin {adminId}");
                return Core.Models.Result.Result.Fail("no_session", "No Session");
            }

            session.EndType = SessionEndTypes.AdminLogout;

            bool removeSessionResult = _sessionRepository.Remove(session);
            if (!removeSessionResult)
            {
                _logger.LogError($"Faild to remove session. SessionId {request.SessionId}, Admin {adminId}");
                return Core.Models.Result.Result.Fail("error", "Error");
            }

            _logger.LogInformation($"Sessions logout. UserId {request.SessionId}, Admin {adminId}");

            return Core.Models.Result.Result.Ok();
        }

        public async Task<Core.Models.Result.Result> LogoutUser(LogoutUserSessionsRequest request, string adminId)
        {
            ValidationResult validationResult = _logoutUserSessionsValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning($"Invlid LogoutSessionRequest. Admin {adminId}");
                return Core.Models.Result.Result.Fail(validationResult.Errors);
            }

            AppUserEntity appUser = await _userManager.FindByIdAsync(request.UserId);
            if (appUser == null)
            {
                _logger.LogWarning($"No user. UserId {request.UserId}, Admin {adminId}");
                return Core.Models.Result.Result.Fail("no_user", "No user");
            }

            SelectSpecification<SessionEntity, SessionEntity> specification = new SelectSpecification<SessionEntity, SessionEntity>();
            specification.AddFilter(x => x.UserId == request.UserId);
            specification.AddSelect(x => x);

            List<SessionEntity> sessions = _sessionRepository.GetList(specification);
            if(sessions.Count == 0)
            {
                return Core.Models.Result.Result.Ok();
            }

            foreach(SessionEntity session in sessions)
            {
                session.EndType = SessionEndTypes.AdminLogout;
            }

            bool removeResult = await _sessionRepository.Remove(sessions);
            if (!removeResult)
            {
                _logger.LogWarning($"Faild to remove sessions. UserId {request.UserId}, Admin {adminId}");
                return Core.Models.Result.Result.Fail("error", "Error");
            }

            _logger.LogInformation($"User session logout. Admin {adminId}");

            return Core.Models.Result.Result.Ok();
        }


        public Core.Models.Result.Result Logout(string sessionCode, string userId, SessionEndTypes endType)
        {
            BaseSpecification<SessionEntity> specification = new BaseSpecification<SessionEntity>();
            specification.AddFilter(x => x.Code == sessionCode);
            specification.AddFilter(x => x.UserId == userId);

            SessionEntity session = _sessionRepository.SingleOrDefault(specification);
            if (session == null)
            {
                _logger.LogError($"Sesion does not exist. SessionCode {sessionCode}, userId {userId}");
                return Core.Models.Result.Result.Fail("no_session", "No Session");
            }

            session.EndType = endType;

            bool removeSessionResult = _sessionRepository.Remove(session);
            if (!removeSessionResult)
            {
                _logger.LogError($"Faild to remove session. SessionCode {sessionCode}, userId {userId}");
                return Core.Models.Result.Result.Fail("error", "Error");
            }

            _logger.LogInformation($"User session logout. UserId {userId}");

            return Core.Models.Result.Result.Ok();
        }

        public bool Validate(string sessionCode, string userId, string ip)
        {
            BaseSpecification<SessionEntity> specification = new BaseSpecification<SessionEntity>();
            specification.AddFilter(x => x.Code == sessionCode);
            specification.AddFilter(x => x.UserId == userId);

            SessionEntity session = _sessionRepository.SingleOrDefault(specification);
            if(session == null)
            {
                return false;
            }

            session.Ip = ip;
            session.LastAccess = DateTimeOffset.UtcNow;

            return _sessionRepository.Update(session);
        }

        public Core.Models.Result.Result Add(string code, string userId, string ip)
        {
            SessionEntity session = new SessionEntity(
                ip: ip,
                userId: userId,
                code: code);

            bool addResult = _sessionRepository.Add(session);
            if(!addResult)
            {
                _logger.LogError($"Failed to add session. UserId {userId}");
                return Core.Models.Result.Result.Fail("error", "Error");
            }

            return Core.Models.Result.Result.Ok();
        }

        public Task<Result> Add(string code, string userId)
        {
            string remoteIp = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

            Core.Models.Result.Result result = Add(code, userId, remoteIp);

            return Task.FromResult(result.ToNewResult());
        }
    }
}
