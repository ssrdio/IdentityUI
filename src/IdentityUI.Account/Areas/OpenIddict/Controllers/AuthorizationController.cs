//https://github.com/openiddict/openiddict-core/blob/dev/samples/Mvc.Server/Controllers/AuthorizationController.cs

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenIddict.Abstractions;
using Microsoft.AspNetCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Authorization;
using OpenIddict.Server.AspNetCore;
using Microsoft.Extensions.Logging;
using OpenIddict.Core;
using SSRD.IdentityUI.Core.Data.Entities.OpenIdConnect;

namespace SSRD.IdentityUI.Account.Areas.OpenIddict
{
    [Area("OpenIddict")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AuthorizationController : Controller
    {
        private const string OPENID_REQUEST_NOT_FOUND = "openid_request_not_found";
        private const string USER_NOT_FOUND = "user_not_found";
        private const string CLIENT_NOT_FOUND = "client_not_found";

        private readonly OpenIddictApplicationManager<ClientEntity> _applicationManager;
        private readonly OpenIddictAuthorizationManager<ClientConsentEntity> _authorizationManager;
        private readonly OpenIddictScopeManager<ClientScopeEntity> _scopeManager;
        private readonly SignInManager<AppUserEntity> _signInManager;
        private readonly UserManager<AppUserEntity> _userManager;

        private readonly ILogger<AuthorizationController> _logger;

        public AuthorizationController(
            OpenIddictApplicationManager<ClientEntity> applicationManager,
            OpenIddictAuthorizationManager<ClientConsentEntity> authorizationManager,
            OpenIddictScopeManager<ClientScopeEntity> scopeManager,
            SignInManager<AppUserEntity> signInManager,
            UserManager<AppUserEntity> userManager,
            ILogger<AuthorizationController> logger)
        {
            _applicationManager = applicationManager;
            _authorizationManager = authorizationManager;
            _scopeManager = scopeManager;
            _signInManager = signInManager;
            _userManager = userManager;

            _logger = logger;
        }

        #region Authorization code, implicit and hybrid flows
        // Note: to support interactive flows like the code flow,
        // you must provide your own authorization endpoint action:

        private IActionResult Error(string error)
        {
            return BadRequest();
        }

        [HttpGet("~/connect/authorize")]
        [HttpPost("~/connect/authorize")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Authorize()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            // Retrieve the user principal stored in the authentication cookie.
            // If it can't be extracted, redirect the user to the login page.
            AuthenticateResult result = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
            if (result is null || !result.Succeeded)
            {
                // If the client application requested promptless authentication,
                // return an error indicating that the user is not logged in.
                if (request.HasPrompt(Prompts.None))
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is not logged in."
                        }));
                }

                return Challenge(
                    authenticationSchemes: IdentityConstants.ApplicationScheme,
                    properties: new AuthenticationProperties
                    {
                        RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                            Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList())
                    });
            }

            // If prompt=login was specified by the client application,
            // immediately return the user agent to the login page.
            if (request.HasPrompt(Prompts.Login))
            {
                // To avoid endless login -> authorization redirects, the prompt=login flag
                // is removed from the authorization request payload before redirecting the user.
                var prompt = string.Join(" ", request.GetPrompts().Remove(Prompts.Login));

                var parameters = Request.HasFormContentType ?
                    Request.Form.Where(parameter => parameter.Key != Parameters.Prompt).ToList() :
                    Request.Query.Where(parameter => parameter.Key != Parameters.Prompt).ToList();

                parameters.Add(KeyValuePair.Create(Parameters.Prompt, new StringValues(prompt)));

                return Challenge(
                    authenticationSchemes: IdentityConstants.ApplicationScheme,
                    properties: new AuthenticationProperties
                    {
                        RedirectUri = Request.PathBase + Request.Path + QueryString.Create(parameters)
                    });
            }

            // If a max_age parameter was provided, ensure that the cookie is not too old.
            // If it's too old, automatically redirect the user agent to the login page.
            if (request.MaxAge.HasValue && result.Properties != null && result.Properties.IssuedUtc.HasValue &&
                DateTimeOffset.UtcNow - result.Properties.IssuedUtc > TimeSpan.FromSeconds(request.MaxAge.Value))
            {
                if (request.HasPrompt(Prompts.None))
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is not logged in."
                        }));
                }

                return Challenge(
                    authenticationSchemes: IdentityConstants.ApplicationScheme,
                    properties: new AuthenticationProperties
                    {
                        RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                            Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList())
                    });
            }

            // Retrieve the profile of the logged in user.
            var user = await _userManager.GetUserAsync(result.Principal) ??
                throw new InvalidOperationException("The user details cannot be retrieved.");

            // Retrieve the application details from the database.
            var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
                throw new InvalidOperationException("Details concerning the calling client application cannot be found.");

            // Retrieve the permanent authorizations associated with the user and the calling client application.
            var authorizations = await _authorizationManager
                .FindAsync(
                    subject: await _userManager.GetUserIdAsync(user),
                    client: await _applicationManager.GetIdAsync(application),
                    status: Statuses.Valid,
                    type: AuthorizationTypes.Permanent,
                    scopes: request.GetScopes())
                .ToListAsync();

            switch (await _applicationManager.GetConsentTypeAsync(application))
            {
                // If the consent is external (e.g when authorizations are granted by a sysadmin),
                // immediately return an error if no authorization can be found in the database.
                case ConsentTypes.External when !authorizations.Any():
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                                "The logged in user is not allowed to access this client application."
                        }));

                // If the consent is implicit or if an authorization was found,
                // return an authorization response without displaying the consent form.
                case ConsentTypes.Implicit:
                case ConsentTypes.External when authorizations.Any():
                case ConsentTypes.Explicit when authorizations.Any() && !request.HasPrompt(Prompts.Consent):
                    var principal = await _signInManager.CreateUserPrincipalAsync(user);

                    // Note: in this sample, the granted scopes match the requested scope
                    // but you may want to allow the user to uncheck specific scopes.
                    // For that, simply restrict the list of scopes before calling SetScopes.
                    principal.SetScopes(request.GetScopes());
                    principal.SetResources(await _scopeManager.ListResourcesAsync(principal.GetScopes()).ToListAsync());

                    // Automatically create a permanent authorization to avoid requiring explicit consent
                    // for future authorization or token requests containing the same scopes.
                    var authorization = authorizations.LastOrDefault();
                    if (authorization is null)
                    {
                        OpenIddictAuthorizationDescriptor openIddictAuthorizationDescriptor = new OpenIddictAuthorizationDescriptor
                        {
                            Principal = principal,
                            Subject = await _userManager.GetUserIdAsync(user),
                            ApplicationId = await _applicationManager.GetIdAsync(application),
                            Type = AuthorizationTypes.Permanent,
                            CreationDate = DateTimeOffset.UtcNow,
                            Status = Statuses.Valid,
                        };

                        openIddictAuthorizationDescriptor.Scopes.UnionWith(principal.GetScopes());

                        authorization = await _authorizationManager.CreateAsync(openIddictAuthorizationDescriptor);
                    }

                    principal.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorization));

                    foreach (var claim in principal.Claims)
                    {
                        claim.SetDestinations(GetDestinations(claim, principal));
                    }

                    return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                // At this point, no authorization was found in the database and an error must be returned
                // if the client application specified prompt=none in the authorization request.
                case ConsentTypes.Explicit when request.HasPrompt(Prompts.None):
                case ConsentTypes.Systematic when request.HasPrompt(Prompts.None):
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                                "Interactive user consent is required."
                        }));

                // In every other case, render the consent form.
                default:
                    return View(new AuthorizeViewModel
                    {
                        ApplicationName = await _applicationManager.GetLocalizedDisplayNameAsync(application),
                        Scope = request.Scope
                    });
            }
        }

        [Authorize, FormValueRequired("submit.Accept")]
        [HttpPost("~/connect/authorize"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept()
        {
            OpenIddictRequest request = HttpContext.GetOpenIddictServerRequest();
            if(request is null)
            {
                _logger.LogError($"Openid connect request not found");
                return Error(OPENID_REQUEST_NOT_FOUND);
            }

            AppUserEntity user = await _userManager.GetUserAsync(User);
            if(user is null)
            {
                _logger.LogError($"User could not be found. Request {request.RequestId}");
                return Error(USER_NOT_FOUND);
                
            }

            ClientEntity client = await _applicationManager.FindByClientIdAsync(request.ClientId);
            if(client is null)
            {
                _logger.LogError($"Client not found. Request {request.RequestId}");
                return Error(CLIENT_NOT_FOUND);
            }

            List<ClientConsentEntity> authorizations = await _authorizationManager
                .FindAsync(
                    subject: user.Id,
                    client: client.Id,
                    status: Statuses.Valid,
                    type: AuthorizationTypes.Permanent,
                    scopes: request.GetScopes())
                .ToListAsync();

            // Note: the same check is already made in the other action but is repeated
            // here to ensure a malicious user can't abuse this POST-only endpoint and
            // force it to return a valid response without the external authorization.
            if (!authorizations.Any() && await _applicationManager.HasConsentTypeAsync(client, ConsentTypes.External))
            {
                _logger.LogWarning($"User is not allowed to access this client. UserId {user.Id}, ClientId {client.Id}");

                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The logged in user is not allowed to access this client application."
                    }));
            }

            ClaimsPrincipal principal = await _signInManager.CreateUserPrincipalAsync(user);

            // Note: in this sample, the granted scopes match the requested scope
            // but you may want to allow the user to uncheck specific scopes.
            // For that, simply restrict the list of scopes before calling SetScopes.
            principal.SetScopes(request.GetScopes());
            principal.SetResources(await _scopeManager.ListResourcesAsync(principal.GetScopes()).ToListAsync());

            // Automatically create a permanent authorization to avoid requiring explicit consent
            // for future authorization or token requests containing the same scopes.
            ClientConsentEntity authorization = authorizations.LastOrDefault();
            if (authorization is null)
            {
                authorization = await _authorizationManager.CreateAsync(
                    principal: principal,
                    subject: user.Id,
                    client: client.Id,
                    type: AuthorizationTypes.Permanent,
                    scopes: principal.GetScopes());
            }

            principal.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorization));

            foreach (Claim claim in principal.Claims)
            {
                claim.SetDestinations(GetDestinations(claim, principal));
            }

            // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        [Authorize, FormValueRequired("submit.Deny")]
        [HttpPost("~/connect/authorize"), ValidateAntiForgeryToken]
        // Notify OpenIddict that the authorization grant has been denied by the resource owner
        // to redirect the user agent to the client application using the appropriate response_mode.
        public async Task<IActionResult> Deny()
        {
            OpenIddictRequest request = HttpContext.GetOpenIddictServerRequest();
            if (request is null)
            {
                _logger.LogError($"Openid connect request not found");
                return Error(OPENID_REQUEST_NOT_FOUND);
            }

            AppUserEntity user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                _logger.LogError($"User could not be found. Request {request.RequestId}");
                return Error(USER_NOT_FOUND);

            }

            ClientEntity client = await _applicationManager.FindByClientIdAsync(request.ClientId);
            if (client is null)
            {
                _logger.LogError($"Client not found. Request {request.RequestId}");
                return Error(CLIENT_NOT_FOUND);
            }

            _logger.LogInformation($"User has denied ");

            return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        #endregion

        #region Logout support for interactive flows like code and implicit
        // Note: the logout action is only useful when implementing interactive
        // flows like the authorization code flow or the implicit flow.

        [HttpGet("~/connect/logout")]
        public IActionResult Logout() => View();

        [ActionName(nameof(Logout)), HttpPost("~/connect/logout"), ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutPost()
        {
            // Ask ASP.NET Core Identity to delete the local and external cookies created
            // when the user agent is redirected from the external identity provider
            // after a successful authentication flow (e.g Google or Facebook).
            await _signInManager.SignOutAsync();

            // Returning a SignOutResult will ask OpenIddict to redirect the user agent
            // to the post_logout_redirect_uri specified by the client application or to
            // the RedirectUri specified in the authentication properties if none was set.
            return SignOut(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = "/"
                });
        }
        #endregion

        #region Password, authorization code, device and refresh token flows
        // Note: to support non-interactive flows like password,
        // you must provide your own token endpoint action:

        [HttpPost("~/connect/token"), Produces("application/json")]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            if (request.IsPasswordGrantType())
            {
                var user = await _userManager.FindByNameAsync(request.Username);
                if (user is null)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The username/password couple is invalid."
                        }));
                }

                // Validate the username/password parameters and ensure the account is not locked out.
                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
                if (!result.Succeeded)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The username/password couple is invalid."
                        }));
                }

                var principal = await _signInManager.CreateUserPrincipalAsync(user);

                // Note: in this sample, the granted scopes match the requested scope
                // but you may want to allow the user to uncheck specific scopes.
                // For that, simply restrict the list of scopes before calling SetScopes.
                principal.SetScopes(request.GetScopes());
                principal.SetResources(await _scopeManager.ListResourcesAsync(principal.GetScopes()).ToListAsync());

                foreach (var claim in principal.Claims)
                {
                    claim.SetDestinations(GetDestinations(claim, principal));
                }

                // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            else if (request.IsAuthorizationCodeGrantType() || request.IsDeviceCodeGrantType() || request.IsRefreshTokenGrantType())
            {
                // Retrieve the claims principal stored in the authorization code/device code/refresh token.
                var principal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;

                // Retrieve the user profile corresponding to the authorization code/refresh token.
                // Note: if you want to automatically invalidate the authorization code/refresh token
                // when the user password/roles change, use the following line instead:
                // var user = _signInManager.ValidateSecurityStampAsync(info.Principal);
                var user = await _userManager.GetUserAsync(principal);
                if (user is null)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The token is no longer valid."
                        }));
                }

                // Ensure the user is still allowed to sign in.
                if (!await _signInManager.CanSignInAsync(user))
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is no longer allowed to sign in."
                        }));
                }

                foreach (var claim in principal.Claims)
                {
                    claim.SetDestinations(GetDestinations(claim, principal));
                }

                // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }
            else if(request.IsClientCredentialsGrantType())
            {
                throw new NotImplementedException();
            }

            throw new InvalidOperationException("The specified grant type is not supported.");
        }
        #endregion

        private IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
        {
            // Note: by default, claims are NOT automatically included in the access and identity tokens.
            // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
            // whether they should be included in access tokens, in identity tokens or in both.

            switch (claim.Type)
            {
                case Claims.Name:
                    yield return Destinations.AccessToken;

                    if (principal.HasScope(Scopes.Profile))
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Email:
                    yield return Destinations.AccessToken;

                    if (principal.HasScope(Scopes.Email))
                        yield return Destinations.IdentityToken;

                    yield break;

                case Claims.Role:
                    yield return Destinations.AccessToken;

                    if (principal.HasScope(Scopes.Roles))
                        yield return Destinations.IdentityToken;

                    yield break;

                // Never include the security stamp in the access and identity tokens, as it's a secret value.
                case "AspNet.Identity.SecurityStamp": yield break;

                default:
                    yield return Destinations.AccessToken;
                    yield break;
            }
        }
    }
}
