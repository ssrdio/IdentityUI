# IdentityUI
[![stable](https://img.shields.io/nuget/v/SSRD.IdentityUI.svg?label=stable)](https://www.nuget.org/packages/SSRD.IdentityUI/)

[![](https://sonarcloud.io/api/project_badges/measure?project=ssrdio_IdentityUI&branch=master&metric=vulnerabilities)](https://sonarcloud.io/dashboard/?id=ssrdio_IdentityUI&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=ssrdio_IdentityUI&branch=master&metric=bugs)](https://sonarcloud.io/dashboard/?id=ssrdio_IdentityUI&branch=master) 
[![](https://sonarcloud.io/api/project_badges/measure?project=ssrdio_IdentityUI&branch=master&metric=code_smells)](https://sonarcloud.io/dashboard/?id=ssrdio_IdentityUI&branch=master) 


IdentityUI is a simple platform for administrative management of users and admins with a graphical interface. It is easy to set up, has a clean API, and runs on all recent. NET Core releases.

![](images/example.gif)

## Nuget
Install SSRD.IdentityUI [NuGet](https://www.nuget.org/packages/SSRD.IdentityUI/) package.

## AppSettings:

```json
"IdentityUI": {
  "BasePath": "http://localhost:5000",
  "Database": {
    "Type": "PostgreSql",
    "ConnectionString": "UserID={User};Password={Password};Host={IP};Port={Port};Database={DatabaseName};Pooling=true;"
  },
  "EmailSender": {
    "Ip": "{Ip}",
    "Port": "{Port}",
    "Username": "{Username}",
    "Password": "{Password}",
    "SenderEmail": "{Sender}",
    "SenderDisplayName": "{Friendly name}"
  },
  "ReCaptcha": {
    "SiteKey": "{SiteKey}",
    "SiteSecret": "{SiteSecret}"
  }
}
```
EmailSender options are optional if you provide custom implementation of IEmailSender or don't want to use an EmailSender.

## Startup

In the `ConfigureServices` method add:
```csharp
services.ConfigureIdentityUI(Configuration) // Configures IdentityUI. You can pass in your own identityUI options.
    .UsePostgre() // Adds dbContext. You can choose between UsePostgre or UseSqlServer.
    .AddIdentityUI() // Adds IdentityManagement core services.
    .AddAuth() // Adds Authentication. You can pass in your own CookieAuthenticationOptions.
    .AddEmailSender() // Optional if you provide  custom implementation of IEmailSender
    .AddIdentityAdmin() // Adds services for IdentityAdminUI
    .AddAccountManagement(); // Adds services for AccountManagement.
```

In the `Configure` method add:
```c#
app.UseIdentityUI(); // Adds IdentityUI
```

If you are using .NET Core 2 you need to configure the MVC setup by adding or updating:
```c#
  app.UseMvc(routes => 
  {
    /* your code */
    routes.MapAccountManagement(); // Adds AccountManagement UI
    routes.MapIdentityAdmin(); // Adds IdentityAdmin UI
  });
```

You can seed an admin user by calling `app.SeedIdentityAdmin("admin", "Password");`.

**Important: If you are using .NET Core 3 remove `app.UseAuthorization()`;**

IdentityAdmin Dashboard: `{server}:{port}/IdentityAdmin/`
Account management: `{server}:{port}/Account/Manage/`

## Database
Supported databases: PostgreSQL, SQL Server, InMemory (only for testing).

InMemory database provider may not be able translate all the queries and cause exceptions.

To create database:
```c#
serviceProvider.RunIdentityMigrations();
```

To seed IdentityUI required entities:
```c#
serviceProvider.SeedSystemEntities();
```
or 
```c#
serviceProvider.SeedMissingSystemEntities();
```

All of this functions are available as extensions on `IServiceProvider`, `IHost`, `IWebHost` or `IApplicationBuilder`

# Groups
From version 2.0, we are supporting a group/multi-tenant management. For this purpose, we created multiple group roles that are linked to permission inside group/tenant management. 

| Permission   |      Description     | 
|----------|:-------------:|
| group_can_manage_attributes |  Can manage group attributes |
| group_can_remove_users |  Can remove users from group |
| group_can_manage_roles |  User can assign roles inside the group |
| group_can_invite_users |  Can invite new users to this group |
| group_can_manage_invites |  Can see and edit invites |
| group_can_see_users |  User can see other members in group |
| identity_ui_can_manage_groups |  Can add new groups and can edit existing groups |
| group_can_add_existing_users |  Can add existing users. **Note**: This will expose all users from Identity server! |


# Advanced configuration

## Configure IdentityUI 

```c#
ConfigureIdentityUI(Configuration, endpoints => 
{
    endpoints.Home = "/";

    endpoints.Login = "/Account/Login/";
    endpoints.Logout = "/Account/Logout/";
    endpoints.AccessDenied = "/Account/AccessDenied/";

    endpoints.Manage = "/Account/Manage/";
    endpoints.ConfirmeEmail = "/Account/ConfirmEmail";
    endpoints.ResetPassword = "/Account/ResetPassword";

    endpoints.RegisterEnabled = true;
    endpoints.UseEmailSender = false;
}) // These are the default endpoints options.
```

## Identity policy 
```c#
AddIdentityUI(options =>
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
}) // These are the default identity options.
```

## Identity options 

```c#
AddAuth(options => 
{
    options.Cookie.HttpOnly = true;
    options.LoginPath = "/Account/Login/";
    options.AccessDeniedPath = "/Account/AccessDenied/";
    options.SlidingExpiration = true;
    options.LogoutPath = "/Account/Logout/";
}) // These are the default cookie options.
```

## Configuring SMS gateway
To be able to use SMS sending functionality within `IdentityUI` you fill first need to configure the system to communication with your SMS gateway. In this example we will show how the Twilio API can be configured.

First you will need to create a Twilio account. You can do that [here](https://www.twilio.com/try-twilio). When your account is ready, you will need update the `appsettings.json` file with API access token. For example:
```json
"IdentityUI": {
  "SmsGateway": {
    "Sid": "",
    "Token": "",
    "FromNumber": ""
  }
}
```
The names of the property can differ from provider to provider, but in general:
* `Sid` should contain the username/account ID 
* `Token` should contain the password/API access token 
* `FromNumber` should contain the phone number, which is used to send the SMS messages

After updating the `appsettings.json` file, you need to add and implementation of the `ISmsSender` interface to your project. A simple Twilio implementation can look something like this:
```c#
public class TwilioSmsSender : ISmsSender
{
    private readonly PhoneNumber _from;

    public TwilioSmsSender(string sid, string token, string from)
    {
        TwilioClient.Init(sid, token);
        _from = new PhoneNumber(from);
    }

    public Task<Result> Send(string to, string message)
    {
        try
        {
            MessageResource result = MessageResource.Create(
                from: _from,
                to: new PhoneNumber(to),
                body: message);

            return Task.FromResult(Result.Ok());
        }
        catch (Exception ex)
        {
            return Task.FromResult(Result.Fail("twilio_error", "Sending SMS failed"));
        }
    }
}

// add the class to the DI container
services.AddScoped<ISmsSender, TwilioSmsSender>(options =>
{
    string sid = Configuration["IdentityUI:SmsGateway:Sid"];
    string token = Configuration["IdentityUI:SmsGateway:Token"];
    string from = Configuration["IdentityUI:SmsGateway:FromNumber"];

    return new TwilioSmsSender(sid, token, from);
});
```

Finally, you need to tell the system that the sms gateway is configured. To do that, you need to update the configuration in the `Setup.cs` file and adding the following line:

```c#
services.ConfigureIdentityUI(Configuration, endpoints =>
{
  endpoints.UseSmsGateway = true;
})
```

With that, you should have SMS sending functionality available in your system. 

Setting up an SMS gateway also enables SMS two-factor authentication for the users of your system.

# Custom pages
Check out [custom pages documentation](docs/IdentityUI%20custom%20pages.md) and define page structure and styles on your own!

# Support
For custom feature request or technical support contact us at [identity[at]ssrd.io](identity@ssrd.io)

## Credits
- [RoyalUI-Free-Bootstrap-Admin-Template](https://github.com/TemplateWatch/RoyalUI-Free-Bootstrap-Admin-Template)
