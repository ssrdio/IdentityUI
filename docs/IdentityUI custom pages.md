# Custom IdentityUI pages

You can define the IdentityUI page layout according to your wishes. Files have to be placed inside `/Areas/Account/Views/` following by the route path subfolders. Note that if you change any page on your own, you still have to use `</form>` tags for making requests. Usage of script languages (e.g. *Javascript*) is undesirable because of anti-forgery token validation.


## Account
If you want to make your look for pages related to `Login` and `Registration` you should create a folder named **Account**, which will contain `.cshtml` files, each representing its page. For example, if you want to change the layout of the Login page, you need to create a file named `Login.cshtml`. As you can see, folder and file structure has to match with a pathname (e.g. `/Account/Login`) for which you are applying your design.

List of available pages grouped by functionalities:

  * Sign up (`Registration.cshtml`),
  * Sign up successful (`RegisterSuccess.cshtml`),
    * Shown when user register successfully
  * Email confirmed (`ConfirmEmail.cshtml`),
    * Shown when a user clicks on the registration link, received on email
<hr>

  * Sign in (`Login.cshtml`),
  * Sign in using 2FA (`LoginWith2fa.cshtml`),
  * Sign in using recovery code (`LoginWithRecoveryCode.cshtml`),
  * Lockout (`Lockout.cshtml`),
    * Shown after a certain amount of failed access attempts
  * Logout (`Logout.cshtml`),
<hr>

  * Password recovery (`RecoverPassword.cshtml`),
    * Used in case of forgotten password to send *Reset your password* link to email
  * Successful password recovery (`RecoverPasswordSuccess.cshtml`),
  * Reset password (`ResetPassword.cshtml`),
    * Shown when the user clicks on password recovery link, received on email
  * Reset password successful (`ResetPasswordSuccess.cshtml`),
<hr>

  * Access denied (`AccessDenied.cshtml`)
    * Shown when logged in user tries to access the page without privileges


## Manage
To design your own `Profile` or `Change password` pages, which are available once you sign in IdentityUI, you have to create a folder named **Manage**, containing `.cshtml` files. Those two pages are available via `/Account/Manage/` route, following by the file name.

List of available pages:
  * Profile (`Profile.cshtml`),
    * Page used to change *first name*, *last name* and *phone number*
  * Change password (`ChangePassword.cshtml`)


## Two-factor authentication
For changing the structure of 2FA pages inside IdentityUI, you will need to make folder **TwoFactorAuthentication** with files representing *authenticator*, *email*, or *SMS* 2FA pages, which are accessible through `/Account/TwoFactorAuthentication/` path.

List of available pages:
  * Main page (`Index.cshtml`),
    * Landing 2FA page where you can choose between different ways of setting up two factor authentication
  * Two factor authentication (`AddTwoFactorAuthenticator.cshtml`),
    * Page is used for setting up 2FA using authenticator application
  * SMS two factor authentication (`AddTwoFactorPhoneAuthentication.cshtml`),
    * Page is used for setting up 2FA with usage of code received via SMS
  * Email factor authentication (`AddTwoFactorEmailAuthentication.cshtml`)
    * Page is used for setting up 2FA with code received on email
  * Recovery codes (`RecoveryCodesView.cshtml`)
    * Page is used to display recovery codes after successfully setting up 2FA


## Shared layout
Shared components are placed inside **Shared** folder. For example, you can define your own `Header` or `Footer` that are displayed once you are logged in IdentityUI.

List of available components:
  * Main layout (`_ManageLayout.cshtml`)
    * File is used for positioning of *Header*, *Sidebar* and *Body* components and is placed directly in **Shared** folder,
    * It also contains a definition of the document footer section
  * Header (`/Components/ManageHeader/Default.cshtml`)
    * File is representing header of IdentityUI, with *Home* and *Logout* buttons
    * Note that inside **Shared** folder you have to create a folder named **Components**, which then has to contain **ManageHeader** folder, and after that, you can create `Default.cshtml` file
  * Sidebar (`/Components/ManageSidebar/Default.cshtml`)
    * File contains sidebar structure with navigation to IdentityUI subpages
    * Folder structure has to be the same as for *Header* component except for parent folder of `Default.cshtml` file, which has to be named **ManageSidebar**