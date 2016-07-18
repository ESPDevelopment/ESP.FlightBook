using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ESP.FlightBook.Identity.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private readonly AuthMessageSenderOptions _options;
        private readonly EmailTemplates _emailTemplates;
        private readonly IConfigurationRoot _configuration;
        private readonly ILogger _logger;

        public AuthMessageSender(IConfigurationRoot configuration, IOptions<AuthMessageSenderOptions> optionsAccessor, EmailTemplates emailTemplates, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _emailTemplates = emailTemplates;
            _logger = loggerFactory.CreateLogger<AuthMessageSender>();
            _options = optionsAccessor.Value;
        }

        /// <summary>
        /// Sends a confirm email address message to the specified recipient
        /// </summary>
        /// <param name="emailAddress">Email address of the registered user</param>
        /// <param name="confirmUrl">Confirmation url to be contained in the email message</param>
        /// <returns>True if successful, otherwise false</returns>
        public async Task<bool> SendConfirmEmailAsync(string emailAddress, string confirmUrl)
        {
            // Replace tokens with data
            string htmlBody = _emailTemplates.ConfirmEmailTemplateHtml.Replace("{{emailAddress}}", emailAddress);
            htmlBody = htmlBody.Replace("{{confirmUrl}}", confirmUrl);
            string textBody = _emailTemplates.ConfirmEmailTemplateText.Replace("{{emailAddress}}", emailAddress);
            textBody = textBody.Replace("{{confirmUrl}}", confirmUrl);

            // Construct email message
            var message = new SendGridMessage();
            message.AddTo(emailAddress);
            message.From = new MailAddress(_configuration["EmailContent:ConfirmEmailFromEmail"], _configuration["EmailContent:ConfirmEmailFromName"]);
            message.Subject = _configuration["EmailContent:ConfirmEmailSubject"];
            message.Text = textBody;
            message.Html = htmlBody;

            // Send email
            var transport = new SendGrid.Web(_options.SendGridApiKey);
            if (transport != null)
            {
                try
                {
                    await transport.DeliverAsync(message);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Sends a forgot password email message to the specified recipient
        /// </summary>
        /// <param name="emailAddress">Email address of the registered user</param>
        /// <param name="returnUrl">Forgot password url to be contained in the email message</param>
        /// <returns>True if successful, otherwise false</returns>
        public async Task<bool> SendForgotPasswordEmailAsync(string emailAddress, string returnUrl)
        {
            // Replace tokens with data
            string htmlBody = _emailTemplates.ForgotPasswordEmailTemplateHtml.Replace("{{emailAddress}}", emailAddress);
            htmlBody = htmlBody.Replace("{{returnUrl}}", returnUrl);
            string textBody = _emailTemplates.ForgotPasswordEmailTemplateText.Replace("{{emailAddress}}", emailAddress);
            textBody = textBody.Replace("{{returnUrl}}", returnUrl);

            // Construct email message
            var message = new SendGridMessage();
            message.AddTo(emailAddress);
            message.From = new MailAddress(_configuration["EmailContent:ForgotPasswordEmailFromEmail"], _configuration["EmailContent:ForgotPasswordEmailFromName"]);
            message.Subject = _configuration["EmailContent:ForgotPasswordEmailSubject"];
            message.Text = textBody;
            message.Html = htmlBody;

            // Send email
            var transport = new SendGrid.Web(_options.SendGridApiKey);
            if (transport != null)
            {
                try
                {
                    await transport.DeliverAsync(message);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return false;
                }
            }
            return true;
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }

    }
}
