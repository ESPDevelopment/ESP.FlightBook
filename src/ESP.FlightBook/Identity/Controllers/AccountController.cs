using ESP.FlightBook.Identity.Models;
using ESP.FlightBook.Identity.Models.AccountViewModels;
using ESP.FlightBook.Identity.Services;
using ESP.FlightBook.Identity.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ESP.FlightBook.Identity.Controllers
{
    [RequireHttps]
    [Authorize("Bearer")]
    public class AccountController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly ISmsSender _smsSender;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly TokenAuthOptions _tokenAuthOptions;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            TokenAuthOptions tokenAuthOptions,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory)
        {
            _signInManager = signInManager;
            _tokenAuthOptions = tokenAuthOptions;
            _userManager = userManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        /// <summary>
        /// Changes the password for the currenc user's account
        /// </summary>
        /// <param name="model">ChangePasswordViewModel containing current password, new password, and password confirmation values.</param>
        /// <returns>
        /// (200) Ok - Account password change succeeded
        /// (400) Bad Request - Input values are not valid
        /// (409) Conflict - Account password change failed
        /// </returns>
        [HttpPost("api/v1/account/changePassword", Name = "PostChangePasswordRoute")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            // Validate model
            if (!ModelState.IsValid)
            {
                if (model == null) model = new ChangePasswordViewModel();
                model.OldPassword = "***";
                model.NewPassword = "***";
                model.ConfirmPassword = "***";
                model.Succeeded = false;
                model.Message = "Invalid email address or password.";
                return BadRequest(model);
            }

            // Get currenct user
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                model.OldPassword = "***";
                model.NewPassword = "***";
                model.ConfirmPassword = "***";
                model.Succeeded = false;
                model.Message = "You must be logged in to change your password.";
                return StatusCode(409, model);
            }

            // Attempt to change password
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                LogIdentityErrors(3, "PostChangePasswordRoute", result);
                AppendErrors(result, model);
                model.OldPassword = "***";
                model.NewPassword = "***";
                model.ConfirmPassword = "***";
                model.Succeeded = false;
                model.Message = "Unable to change password.";
                return StatusCode(409, model);
            }

            // Signin the user
            await _signInManager.SignInAsync(user, isPersistent: false);

            // Return success
            model.OldPassword = "***";
            model.NewPassword = "***";
            model.ConfirmPassword = "***";
            model.Succeeded = true;
            model.Message = "Password changed successfully.";
            return Ok(model);
        }

        /// <summary>
        /// Confirms the email address for a new account
        /// </summary>
        /// <param name="model">ConfirmEmailViewModel containing the user id and code values.</param>
        /// <returns>
        /// (200) Ok - Account confirmation succeeded
        /// (400) Bad Request - Input values are not valid
        /// (409) Conflict - Account confirmation failed
        /// </returns>
        [HttpPost("api/v1/account/confirmEmail", Name = "PostConfirmEmailRoute")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromBody]ConfirmEmailViewModel model)
        {
            // Validate input
            if (!ModelState.IsValid)
            {
                if (model == null) model = new ConfirmEmailViewModel();
                model.Succeeded = false;
                model.Message = "Invalid user or confirmation code.";
                return BadRequest(model);

            }

            // Locate account to be validated
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return StatusCode(409, new { Succeeded = false, Message = "Invalid user or confirmation code." });
            }

            // Confirm the account
            var result = await _userManager.ConfirmEmailAsync(user, model.Code);
            if (!result.Succeeded)
            {
                model.Succeeded = false;
                model.Message = "Invalid user or confirmation code.";
                return StatusCode(409, model);
            }

            // Return success
            model.Succeeded = true;
            model.Message = "Email address successfully confirmed.";
            return Ok(model);
        }

        /// <summary>
        /// Generate password recovery email
        /// </summary>
        /// <param name="model">ForgotPasswordViewModel containing email address and return Url value.</param>
        /// <returns>
        /// (200) Ok - Password recovery email generation succeeded
        /// (400) Bad Request - Input values not valid
        /// (409) Conflict - Password recovery email generation failed
        /// </returns>
        [HttpPost("api/v1/account/forgotPassword", Name = "PostForgotPasswordRoute")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
        {
            // Validate the model
            if (!ModelState.IsValid)
            {
                if (model == null) model = new ForgotPasswordViewModel();
                model.Succeeded = false;
                model.Message = "Unable to initiate password reset.";
                return BadRequest(model);
            }

            // Locate account
            var user = await _userManager.FindByNameAsync(model.EmailAddress);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                model.Succeeded = false;
                model.Message = "Unable to initiate password reset.";
                return StatusCode(409, model);
            }

            // Generate password reset token
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Generate password reset uri
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("userId", user.Id));
            parameters.Add(new KeyValuePair<string, string>("code", code));
            QueryString queryString = QueryString.Create(parameters);
            var callbackUrl = model.ReturnUrl + queryString.ToUriComponent();

            // Send password recovery email
            bool succeeded = await _emailSender.SendForgotPasswordEmailAsync(model.EmailAddress, callbackUrl);
            if (!succeeded)
            {
                model.Succeeded = false;
                model.Message = "Unable to send password recovery email.";
                return StatusCode(409, model);
            }

            // Return result
            model.Succeeded = true;
            return Ok(model);
        }

        /// <summary>
        /// Authenticates an existing user account
        /// </summary>
        /// <param name="model">LoginViewModel containing email address, password and remember me values.</param>
        /// <returns>
        /// (200) Ok - User authentication succeeded
        /// (400) Bad Request - Input values not valid
        /// (409) Conflict - User authentication failed
        /// </returns>
        [HttpPost("api/v1/account/login", Name = "PostLoginRoute")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            // Validate the model
            if (!ModelState.IsValid)
            {
                if (model == null) model = new LoginViewModel();
                model.Password = "***";
                model.Succeeded = false;
                model.Message = "Invalid email address or password.";
                return BadRequest(model);
            }

            // Require the user to have a confirmed email before they can log on.
            var user = await _userManager.FindByNameAsync(model.EmailAddress);
            if (user != null)
            {
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    model.Password = "***";
                    model.Succeeded = false;
                    model.Message = "You must confirm your email address to sign in.";
                    return StatusCode(409, model);
                }
            }

            // Attempt to login the user
            var result = await _signInManager.PasswordSignInAsync(model.EmailAddress, model.Password, model.RememberMe, lockoutOnFailure: false);
            model.Succeeded = result.Succeeded;

            // Hide password
            model.Password = "***";

            // Process result
            if (result.Succeeded == false)
            {
                // Set appropriate error message
                if (result.IsLockedOut)
                {
                    model.Message = "This account has been locked out.";
                    return StatusCode(409, model);
                }
                if (result.IsNotAllowed)
                {
                    model.Message = "Signin for this account is not allowed";
                    return StatusCode(409, model);
                }
                if (result.RequiresTwoFactor)
                {
                    model.Message = "Signin for this account requires two-factor authentication.";
                    return StatusCode(409, model);
                }

                // Provide default error message
                model.Message = "Email address and password combination is not valid.";
                return StatusCode(409, model);
            }

            // Construct a security token string
            string token = await GetSecurityTokenAsString(model.EmailAddress);
            if (token.Equals(string.Empty))
            {
                model.Message = "An unexpected error occurred during sign in.";
                return BadRequest(model);
            }

            // Return the access token
            model.Succeeded = true;
            model.Message = "";
            model.AccessToken = token;
            return Ok(model);
        }

        /// <summary>
        /// Logs off the user
        /// </summary>
        /// <returns>
        /// (200) Ok - Logoff succeeded
        /// </returns>
        [AllowAnonymous]
        [HttpPost("api/v1/account/logoff", Name = "PostLogoffRoute")]
        public async Task<IActionResult> LogOff()
        {
            // Logout the user
            await _signInManager.SignOutAsync();
            LogInformation(4, "PostLogoffRoute", "User logged out.");
            return Ok();
        }

        /// <summary>
        /// Returns a new access token by validating the previously provided access token
        /// </summary>
        /// <param name="model">RefreshTokenViewModel containing the previous access token value.</param>
        /// <returns>
        /// (200) Ok - New access token generated
        /// (400) Bad Request - Input values not valid
        /// (409) Conflict - New access token could not be generated
        /// </returns>
        [AllowAnonymous]
        [HttpPost("api/v1/account/refreshToken", Name = "PostRefreshTokenRoute")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenViewModel model)
        {
            // Validate the model
            if (!ModelState.IsValid)
            {
                if (model == null) model = new RefreshTokenViewModel();
                model.AccessToken = "***";
                model.Message = "Invalid email address or password.";
                model.NewAccessToken = "***";
                model.Succeeded = false;
                return BadRequest(model);
            }

            // Validate authenticity of previous token
            Microsoft.IdentityModel.Tokens.SecurityToken securityToken;
            ClaimsPrincipal principal = null;
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var validationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateLifetime = false,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    RequireExpirationTime = true,
                    ValidAudience = _tokenAuthOptions.Audience,
                    ValidIssuer = _tokenAuthOptions.Issuer,
                    IssuerSigningKey = _tokenAuthOptions.SigningKey,
                    RequireSignedTokens = true,
                    ValidateIssuerSigningKey = true
                };
                principal = handler.ValidateToken(model.AccessToken, validationParameters, out securityToken);
            }
            catch (Exception e)
            {
                LogInformation(9, "PostRefreshTokenRoute", e.Message);
                model.AccessToken = "***";
                model.Message = "Invalid access token.";
                model.NewAccessToken = "***";
                model.Succeeded = false;
                return StatusCode(409, model);
            }

            // Validate that the account exists
            var user = await _userManager.FindByNameAsync(principal.Identity.Name);
            if (user == null)
            {
                model.AccessToken = "***";
                model.Message = "Unable to generate refresh token.";
                model.NewAccessToken = "***";
                model.Succeeded = false;
                return StatusCode(409, model);
            }

            // Require the user to have a confirmed email and are not locked out
            if (user != null)
            {
                // Check for email-confirmed account
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    model.AccessToken = "***";
                    model.Message = "You must confirm your email address to sign in.";
                    model.NewAccessToken = "***";
                    model.Succeeded = false;
                    return StatusCode(409, model);
                }

                // Check for locked out account
                if (await _userManager.IsLockedOutAsync(user))
                {
                    model.AccessToken = "***";
                    model.Message = "Account has been locked out.";
                    model.NewAccessToken = "***";
                    model.Succeeded = false;
                    return StatusCode(409, model);
                }
            }

            // Construct a security token string
            string token = await GetSecurityTokenAsString(user.Email);
            if (token.Equals(string.Empty))
            {
                model.AccessToken = "***";
                model.Message = "An unexpected error occurred during sign in.";
                model.NewAccessToken = "***";
                model.Succeeded = false;
                return BadRequest(model);
            }

            // Return the access token
            model.Message = "";
            model.NewAccessToken = token;
            model.Succeeded = true;
            return Ok(model);
        }

        /// <summary>
        /// Registers a new account
        /// </summary>
        /// <param name="model">RegisterViewModel containing email address, password, and password confirmation values.</param>
        /// <returns>
        /// (200) Ok - New account was created successfully
        /// (400) Bad Request - Input values not valid
        /// (409) Conflict - New account could not be created
        /// </returns>
        [AllowAnonymous]
        [HttpPost("api/v1/account/register", Name = "PostRegisterRoute")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            // Validate the model
            if (!ModelState.IsValid)
            {
                if (model == null) model = new RegisterViewModel();
                model.Succeeded = false;
                model.Message = "Invalid email address or password.";
                model.Password = "***";
                model.ConfirmPassword = "***";
                return BadRequest(model);
            }

            // Attempt to create a new account
            var user = new ApplicationUser { UserName = model.EmailAddress, Email = model.EmailAddress };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                AppendErrors(result, model);
                model.Succeeded = false;
                model.Message = "Registration failed.";
                model.Password = "***";
                model.ConfirmPassword = "***";
                return StatusCode(409, model);
            }

            // Generate email confirmation token
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Generate password reset uri
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("userId", user.Id));
            parameters.Add(new KeyValuePair<string, string>("code", code));
            QueryString queryString = QueryString.Create(parameters);
            var callbackUrl = model.ConfirmUrl + queryString.ToUriComponent();

            // Send confirmation email
            await _emailSender.SendConfirmEmailAsync(model.EmailAddress, callbackUrl);

            // Return result
            LogInformation(3, "PostRegisterRoute", "User created a new account with password.");
            model.Succeeded = true;
            model.Password = "***";
            model.ConfirmPassword = "***";
            return Ok(model);
        }

        /// <summary>
        /// Resends an email confirmation message
        /// </summary>
        /// <param name="model">ResendConfirmationViewModel containing the email address value.</param>
        /// <returns>
        /// (200) Ok - Email confirmation message sent
        /// (400) Bad Request - Input values not valid
        /// (409) Conflict - Email confirmation message failed
        /// </returns>
        [HttpPost("api/v1/account/resendConfirmationEmail", Name = "PostResendConfirmationEmailRoute")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationViewModel model)
        {
            // Validate the model
            if (!ModelState.IsValid)
            {
                if (model == null) model = new ResendConfirmationViewModel();
                model.Succeeded = false;
                model.Message = "Unable to resend email confirmation message.";
                LogInformation(3, "PostResendConfirmationEmailRoute", "Model is invalid.");
                return BadRequest(model);
            }

            // Locate account
            var user = await _userManager.FindByNameAsync(model.EmailAddress);
            if (user == null)
            {
                model.Succeeded = false;
                model.Message = "Unable to resend email confirmation message.";
                LogInformation(3, "PostResendConfirmationEmailRoute", "Unable to locate account: " + model.EmailAddress + ".");
                return StatusCode(409, model);
            }

            // Generate email confirmation token
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Generate password reset uri
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("userId", user.Id));
            parameters.Add(new KeyValuePair<string, string>("code", code));
            QueryString queryString = QueryString.Create(parameters);
            var callbackUrl = model.ConfirmUrl + queryString.ToUriComponent();

            // Send confirmation email
            await _emailSender.SendConfirmEmailAsync(model.EmailAddress, callbackUrl);

            // Return result
            LogInformation(3, "PostResendConfirmationEmailRoute", "Email confirmation message resent.");
            model.Succeeded = true;
            return Ok(model);
        }

        /// <summary>
        /// Resets an account password
        /// </summary>
        /// <param name="model">ResetPasswordViewModel containing email address, password, password confirmation, and code values.</param>
        /// <returns>
        /// (200) Ok - Password reset succeeded
        /// (400) Bad Request - Input values not valid
        /// (409) Conflict - Password reset failed
        /// </returns>
        [HttpPost("api/v1/account/resetPassword", Name = "PostResetPasswordRoute")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            // Validate the model
            if (!ModelState.IsValid)
            {
                if (model == null) model = new ResetPasswordViewModel();
                model.Succeeded = false;
                model.Message = "Invalid email address, passwords, or reset code.";
                model.Password = "***";
                model.ConfirmPassword = "***";
                model.Code = "***";
                return BadRequest(model);
            }

            // Locate user account
            var user = await _userManager.FindByNameAsync(model.EmailAddress);
            if (user == null)
            {
                model.Succeeded = false;
                model.Message = "Unable to reset password.";
                model.Password = "***";
                model.ConfirmPassword = "***";
                model.Code = "***";
                return StatusCode(409, model);
            }

            // Reset password
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (!result.Succeeded)
            {
                LogIdentityErrors(3, "PostResetPasswordRoute", result);
                AppendErrors(result, model);
                model.Succeeded = false;
                model.Message = "Unable to reset password.";
                model.Password = "***";
                model.ConfirmPassword = "***";
                model.Code = "***";
                return StatusCode(409, model);
            }

            // Return result
            model.Succeeded = true;
            model.Password = "***";
            model.ConfirmPassword = "***";
            model.Code = "***";
            return Ok(model);
        }

        #region Helpers

        /// <summary>
        /// Append identity errors to the view model
        /// </summary>
        /// <param name="result">IdentityResult object</param>
        /// <param name="model">An AccountViewModel object</param>
        private void AppendErrors(IdentityResult result, AccountViewModel model)
        {
            // Append errors to the request model
            if (result != null && result.Errors != null)
            {
                model.Errors = new List<IdentityError>();
                foreach (IdentityError error in result.Errors)
                {
                    model.Errors.Add(error);
                }
            }
        }

        /// <summary>
        /// Return the current user
        /// </summary>
        /// <returns>The current user</returns>
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        /// <summary>
        /// Constructs a JWT security token for the specified username and returns it as
        /// a base-64 encoded string that can be used in an Authorization header.
        /// </summary>
        /// <param name="username">Unique identifier for the user</param>
        /// <returns>A base-64 encoded JWT token if successful, otherwise and empty string</returns>
        private async Task<string> GetSecurityTokenAsString(string username)
        {
            // Retrieve the application user from the identity store
            ApplicationUser user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return string.Empty;
            }

            // Construct an identity with appropriate claims
            IList<Claim> claims = await _userManager.GetClaimsAsync(user);
            if (claims == null)
            {
                return string.Empty;
            }
            claims.Add(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", user.Id));
            claims.Add(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", user.Email));
            ClaimsIdentity identity = new ClaimsIdentity(claims);
            if (identity == null)
            {
                return string.Empty;
            }

            // Create security token
            DateTime issuedAt = DateTime.UtcNow;
            JwtSecurityToken securityToken = new JwtSecurityToken(
                issuer: _tokenAuthOptions.Issuer,
                audience: _tokenAuthOptions.Audience,
                signingCredentials: _tokenAuthOptions.SigningCredentials,
                claims: claims,
                notBefore: issuedAt,
                expires: issuedAt.AddHours(2));
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(securityToken);
        }

        /// <summary>
        /// Log identity action errors
        /// </summary>
        /// <param name="eventId">The event id associated with the error.</param>
        /// <param name="routeName">The name of the route in which the error occurred.</param>
        /// <param name="result">An IdentityResult object.</param>
        private void LogIdentityErrors(int eventId, string routeName, IdentityResult result)
        {
            string uri = Url.RouteUrl(routeName);
            foreach (var error in result.Errors)
            {
                _logger.LogInformation(eventId, "[" + uri + "] " + error.Description);
            }
        }

        /// <summary>
        /// Log information message
        /// </summary>
        /// <param name="eventId">The event id associated with the message.</param>
        /// <param name="routeName">The name of the route from which the message originated.</param>
        /// <param name="message">The message to be logged.</param>
        private void LogInformation(int eventId, string routeName, string message)
        {
            string uri = Url.RouteUrl(routeName);
            _logger.LogInformation(eventId, "[" + uri + "] " + message);
        }

        #endregion
    }
}
