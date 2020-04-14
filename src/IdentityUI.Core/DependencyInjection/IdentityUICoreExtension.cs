using FluentValidation;
using SSRD.IdentityUI.Core.Data.Entities.Identity;
using SSRD.IdentityUI.Core.Data.Models;
using SSRD.IdentityUI.Core.Infrastructure.Data;
using SSRD.IdentityUI.Core.Infrastructure.Data.ReleaseManagment;
using SSRD.IdentityUI.Core.Infrastructure.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Data;
using SSRD.IdentityUI.Core.Interfaces.Data.Repository;
using SSRD.IdentityUI.Core.Interfaces.Services;
using SSRD.IdentityUI.Core.Interfaces.Services.Auth;
using SSRD.IdentityUI.Core.Models;
using SSRD.IdentityUI.Core.Models.Options;
using SSRD.IdentityUI.Core.Services;
using SSRD.IdentityUI.Core.Services.Auth;
using SSRD.IdentityUI.Core.Services.Auth.Email;
using SSRD.IdentityUI.Core.Services.Auth.TwoFactorAuth;
using SSRD.IdentityUI.Core.Services.Identity;
using SSRD.IdentityUI.Core.Services.Role;
using SSRD.IdentityUI.Core.Services.User;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SSRD.IdentityUI.Core.DependencyInjection;
using SSRD.IdentityUI.Core.Infrastructure.Services;

namespace SSRD.IdentityUI.Core
{
    public static class IdentityUICoreExtension
    {
        /// <summary>
        /// Configures IdentityUI
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IdentityUIServicesBuilder ConfigureIdentityUI(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<IdentityUIOptions>(configuration.GetSection("IdentityUI"));
            services.Configure<DatabaseOptions>(configuration.GetSection($"IdentityUI:{nameof(IdentityUIOptions.Database)}"));
            services.Configure<EmailSenderOptions>(configuration.GetSection($"IdentityUI:{nameof(IdentityUIOptions.EmailSender)}"));
            services.Configure<IdentityUIEndpoints>(configuration);

            IdentityUIServicesBuilder builder = new IdentityUIServicesBuilder(services, new IdentityUIEndpoints());

            return builder;
        }

        /// <summary>
        /// Configures IdentityUI
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="endpointAction"></param>
        /// <returns></returns>
        public static IdentityUIServicesBuilder ConfigureIdentityUI(this IServiceCollection services, IConfiguration configuration,
            Action<IdentityUIEndpoints> endpointAction)
        {
            IdentityUIOptions identityUIOptions = configuration.GetSection("IdentityUI").Get<IdentityUIOptions>();

            if(identityUIOptions == null)
            {
                identityUIOptions = new IdentityUIOptions();
            }

            services.Configure<IdentityUIOptions>(options => 
            {
                options.BasePath = identityUIOptions.BasePath;
                options.Database = identityUIOptions.Database;
                options.EmailSender = identityUIOptions.EmailSender;
            });

            services.Configure<DatabaseOptions>(options => 
            {
                options.ConnectionString = identityUIOptions.Database?.ConnectionString;
            });

            services.Configure<EmailSenderOptions>(options => 
            {
                options.Ip = identityUIOptions.EmailSender?.Ip;
                options.Port = identityUIOptions.EmailSender?.Port ?? -1;
                options.UserName = identityUIOptions.EmailSender?.UserName;
                options.Password = identityUIOptions.EmailSender?.Password;
            });

            services.Configure<IdentityUIEndpoints>(endpointAction);

            IdentityUIEndpoints identityManagementEndpoints = new IdentityUIEndpoints();
            endpointAction?.Invoke(identityManagementEndpoints);

            if ((identityUIOptions.EmailSender == null || string.IsNullOrEmpty(identityUIOptions.EmailSender.Ip)) && !identityManagementEndpoints.UseEmailSender)
            {
                identityManagementEndpoints.UseEmailSender = false;
            }
            else
            {
                identityManagementEndpoints.UseEmailSender = true;
            }

            IdentityUIServicesBuilder builder = new IdentityUIServicesBuilder(services, identityManagementEndpoints);

            builder.Services.AddScoped<IEmailSender, NullEmailSender>();

            return builder;
        }

        /// <summary>
        /// Registers IdentityUI services
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="identityOptions"></param>
        /// <returns></returns>
        public static IdentityUIServicesBuilder AddIdentityUI(this IdentityUIServicesBuilder builder,
            Action<IdentityOptions> identityOptions)
        {
            builder.Services.AddDbContext<IdentityDbContext>((provider, options) =>
            {
                DatabaseOptions databaseOptions = provider.GetRequiredService<IOptionsSnapshot<DatabaseOptions>>().Value;
                if(databaseOptions == null)
                {
                    throw new Exception("No DatabaseOptions");
                }
                
                switch(databaseOptions.Type)
                {
                    case DatabaseTypes.PostgreSql:
                        {
                            options.UseNpgsql(databaseOptions.ConnectionString);

                            break;
                        }
                    case DatabaseTypes.InMemory:
                        {
                            options.UseInMemoryDatabase(databaseOptions.ConnectionString);
                            break;
                        }
                    default:
                        {
                            throw new Exception($"Unsupported Database: {databaseOptions.Type}");
                        }
                }
            });

            builder.Services.AddIdentity<AppUserEntity, RoleEntity>()
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            builder.AddValidators();
            builder.AddRepositories();
            builder.AddServices();

            builder.Services.Configure<IdentityOptions>(identityOptions);
            builder.Services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromMinutes(1);
                options.OnRefreshingPrincipal = context =>
                {
                    Claim sessionCode = context.CurrentPrincipal.FindFirst(IdentityManagementClaims.SESSION_CODE);
                    if (sessionCode != null)
                    {
                        context.NewPrincipal.Identities.First().AddClaim(sessionCode);
                    }

                    return Task.CompletedTask;
                };
            });


            return builder;
        }

        /// <summary>
        /// Registers IdentityUI services
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IdentityUIServicesBuilder AddIdentityUI(this IdentityUIServicesBuilder builder)
        {
            Action<IdentityOptions> identityOptions = new Action<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;

                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            });

            return builder.AddIdentityUI(identityOptions);
        }

        /// <summary>
        /// Registers EmailSender
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IdentityUIServicesBuilder AddEmailSender(this IdentityUIServicesBuilder builder)
        {
            builder.Services.AddScoped<IEmailSender, EmailSender>();

            return builder;
        }

        /// <summary>
        /// Configures AuthenticationCookie
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IdentityUIServicesBuilder AddAuth(this IdentityUIServicesBuilder builder)
        {
            Action<CookieAuthenticationOptions> cookieAuthenticationOptions = new Action<CookieAuthenticationOptions>(options =>
            {
                options.Cookie.HttpOnly = true;
                options.LoginPath = builder.IdentityManagementEndpoints.Login;
                options.AccessDeniedPath = builder.IdentityManagementEndpoints.AccessDenied;
                options.SlidingExpiration = true;
                options.LogoutPath = builder.IdentityManagementEndpoints.Logout;
            });

            builder.AddAuth(cookieAuthenticationOptions);

            return builder;
        }

        /// <summary>
        /// Configures AuthenticationCookie
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="optionsAction"></param>
        /// <returns></returns>
        public static IdentityUIServicesBuilder AddAuth(this IdentityUIServicesBuilder builder, Action<CookieAuthenticationOptions> optionsAction)
        {
            builder.Services.ConfigureApplicationCookie(optionsAction);

            return builder;
        }

        private static void AddRepositories(this IdentityUIServicesBuilder builder)
        {
            builder.Services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddTransient<IRoleRepository, RoleRepository>();
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<IUserRoleRepository, UserRoleRepository>();

            builder.Services.AddTransient<ISessionRepository, SessionRepository>();
        }

        private static void AddServices(this IdentityUIServicesBuilder builder)
        {
            builder.Services.AddScoped<IManageUserService, ManageUserService>();
            builder.Services.AddScoped<IAddUserService, AddUserService>();

            builder.Services.AddScoped<IRoleService, RoleService>();

            builder.Services.AddScoped<ILoginService, LoginService>();
            builder.Services.AddScoped<ITwoFactorAuthService, TwoFactorAuthService>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddScoped<ICredentialsService, Core.Services.Auth.Credentials.CredentialsService>();

            builder.Services.AddScoped<ISessionService, Services.Auth.Session.SessionService>();
            builder.Services.AddScoped<IUserClaimsPrincipalFactory<AppUserEntity>, CustomClaimsPrincipalFactory>();
            builder.Services.AddScoped<ISecurityStampValidator, CustomSecurityStampValidator>();
        }

        private static void AddValidators(this IdentityUIServicesBuilder builder)
        {
            builder.Services.AddSingleton<IValidator<Core.Services.User.Models.EditUserRequest>, Core.Services.User.Models.EditUserRequestValidator>();
            builder.Services.AddSingleton<IValidator<Core.Services.User.Models.SetNewPasswordRequest>, Core.Services.User.Models.SetNewPasswordValidator>();

            builder.Services.AddSingleton<IValidator<Core.Services.User.Models.NewUserRequest>, Core.Services.User.Models.NewUserRequestValidator>();
            builder.Services.AddSingleton<IValidator<Core.Services.User.Models.RegisterRequest>, Core.Services.User.Models.RegisterRequestValidator>();

            builder.Services.AddSingleton<IValidator<Core.Services.Auth.Login.Models.LoginRequest>, Core.Services.Auth.Login.Models.LoginRequestValidator>();
            builder.Services.AddSingleton<IValidator<Core.Services.Auth.Login.Models.LoginWith2faRequest>, Core.Services.Auth.Login.Models.LoginWith2faRequestValidator>();

            builder.Services.AddSingleton<IValidator<Core.Services.Auth.TwoFactorAuth.Models.AddTwoFactorAuthenticatorRequest>, Core.Services.Auth.TwoFactorAuth.Models.AddTwoFactorAuthicatorValidator>();

            builder.Services.AddSingleton<IValidator<Core.Services.Role.Models.NewRoleRequest>, Core.Services.Role.Models.NewRoleValidator>();
            builder.Services.AddSingleton<IValidator<Core.Services.Role.Models.EditRoleRequest>, Core.Services.Role.Models.EditRoleValidator>();

            builder.Services.AddSingleton<IValidator<Core.Services.Auth.Credentials.Models.RecoverPasswordRequest>, Core.Services.Auth.Credentials.Models.RecoverPasswordValidator>();
            builder.Services.AddSingleton<IValidator<Core.Services.Auth.Credentials.Models.ResetPasswordRequest>, Core.Services.Auth.Credentials.Models.ResetPasswordValidator>();
            builder.Services.AddSingleton<IValidator<Core.Services.Auth.Credentials.Models.ChangePasswordRequest>, Core.Services.Auth.Credentials.Models.ChangePasswordValidator>();

            builder.Services.AddSingleton<IValidator<Core.Services.User.Models.EditProfileRequest>, Core.Services.User.Models.EditProfileValidator>();

            builder.Services.AddSingleton<IValidator<Core.Services.User.Models.UnlockUserRequest>, Core.Services.User.Models.UnlockUserValidator>();
            builder.Services.AddSingleton<IValidator<Core.Services.User.Models.SendEmailVerificationMailRequest>, Core.Services.User.Models.SendEmailVerificationMailValidtor>();

            builder.Services.AddSingleton<IValidator<Core.Services.Auth.Session.Models.LogoutSessionRequest>, Core.Services.Auth.Session.Models.LogoutSessionValidator>();
            builder.Services.AddSingleton<IValidator<Core.Services.Auth.Session.Models.LogoutUserSessionsRequest>, Core.Services.Auth.Session.Models.LogoutUserSessionValidator>();
        }

        /// <summary>
        /// Adds IdentityUI to the specified Microsoft.AspNetCore.Builder.IApplicationBuilder
        /// </summary>
        /// <param name="app"></param>
        /// <param name="enableMigrations">Flag indicating if migrations should be run</param>
        /// <returns></returns>
        public static IdentityUIAppBuilder UseIdentityUI(this IApplicationBuilder app, bool enableMigrations = false)
        {
            app.UseAuthentication();
#if NET_CORE3
            app.UseAuthorization();
#endif

            if (enableMigrations)
            {
                app.ApplyIdentityMigrations();
            }

            return new IdentityUIAppBuilder(app); 
        }
    }
}
