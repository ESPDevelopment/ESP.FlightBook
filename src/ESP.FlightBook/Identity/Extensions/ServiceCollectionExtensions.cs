using ESP.FlightBook.Identity.Services;
using ESP.FlightBook.Identity.Token;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ESP.FlightBook.Identity.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private const string TokenAudience = "http://espdevelopment.com";
        private const string TokenIssuer = "http://espdevelopment.com";

        public static IServiceCollection AddEmailTemplates(this IServiceCollection services, IHostingEnvironment env)
        {
            // Create email template object
            EmailTemplates emailTemplates = new EmailTemplates();

            // Load email confirmation templates
            string confirmEmailTemplateHtmlPath = env.WebRootPath + "\\email\\confirmEmail.html";
            emailTemplates.ConfirmEmailTemplateHtml = System.IO.File.ReadAllText(confirmEmailTemplateHtmlPath);
            string confirmEmailTemplateTextPath = env.WebRootPath + "\\email\\confirmEmail.txt";
            emailTemplates.ConfirmEmailTemplateText = System.IO.File.ReadAllText(confirmEmailTemplateTextPath);

            // Load reset password templates
            string forgotPasswordEmailTemplateHtmlPath = env.WebRootPath + "\\email\\forgotPasswordEmail.html";
            emailTemplates.ForgotPasswordEmailTemplateHtml = System.IO.File.ReadAllText(forgotPasswordEmailTemplateHtmlPath);
            string forgotPasswordEmailTemplateTextPath = env.WebRootPath + "\\email\\forgotPasswordEmail.txt";
            emailTemplates.ForgotPasswordEmailTemplateText = System.IO.File.ReadAllText(forgotPasswordEmailTemplateTextPath);

            // Create email template singleton
            services.AddSingleton<EmailTemplates>(emailTemplates);

            // Return result
            return services;
        }

        public static IServiceCollection AddESPTokenKey(this IServiceCollection services, IConfigurationRoot configuration)
        {
            // Create security key
            RsaSecurityKey key = RSAKeyUtils.GetDecodedKey(configuration["TokenAuth:Secret"]);

            // Create token auth options
            TokenAuthOptions tokenAuthOptions = new TokenAuthOptions()
            {
                Audience = TokenAudience,
                Issuer = TokenIssuer,
                SigningKey = key,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature)
            };

            // Add token auth instance
            services.AddSingleton<TokenAuthOptions>(tokenAuthOptions);

            // Return result
            return services;
        }
    }
}
