using ESP.FlightBook.Identity.Token;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;

namespace ESP.FlightBook.Identity.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseESPExceptionHandler(this IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            // Register a simple error handler to catch token expiries and change them to a 401, 
            // and return all other errors as a 500.
            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Use(async (context, next) =>
                {
                    var error = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;
                    if (error != null)
                    {
                        var logger = loggerFactory.CreateLogger("ESP.ExceptionHandler");
                        if (error.Error is ArgumentException ||
                            error.Error is JsonReaderException ||
                            error.Error is SecurityTokenExpiredException ||
                            error.Error is SecurityTokenInvalidAudienceException ||
                            error.Error is SecurityTokenInvalidIssuerException ||
                            error.Error is SecurityTokenInvalidLifetimeException ||
                            error.Error is SecurityTokenInvalidSignatureException ||
                            error.Error is SecurityTokenInvalidSigningKeyException ||
                            error.Error is SecurityTokenNoExpirationException ||
                            error.Error is SecurityTokenNotYetValidException ||
                            error.Error is SecurityTokenSignatureKeyNotFoundException ||
                            error.Error is SecurityTokenValidationException)
                        {
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            logger.LogError(0, error.Error, "Request not authorized; returning 401.");
                            await context.Response.WriteAsync(
                                JsonConvert.SerializeObject(
                                    new { success = false, error = error.Error.Message }));
                        }
                        else if (error.Error != null)
                        {
                            context.Response.StatusCode = 500;
                            context.Response.ContentType = "application/json";
                            logger.LogError(0, error.Error, "Unhandled exception; returning 500.");
                            await context.Response.WriteAsync(
                                JsonConvert.SerializeObject
                                (new { success = false, errorType = error.Error.GetType(), error = error.Error.Message }));
                        }
                    }
                    // We're not trying to handle anything else so just let the default 
                    // handler handle.
                    else await next();
                });
            });

            return app;
        }

        public static IApplicationBuilder UseESPTokenAuth(this IApplicationBuilder app, TokenAuthOptions tokenAuthOptions)
        {
            JwtBearerOptions options = new JwtBearerOptions();

            // Basic settings - signing key to validate with, audience and issuer.
            options.TokenValidationParameters.IssuerSigningKey = tokenAuthOptions.SigningKey;
            options.TokenValidationParameters.ValidAudience = tokenAuthOptions.Audience;
            options.TokenValidationParameters.ValidIssuer = tokenAuthOptions.Issuer;

            // When receiving a token, check that we've signed it.
            options.TokenValidationParameters.ValidateIssuerSigningKey = true;

            // When receiving a token, check that it is still valid.
            options.TokenValidationParameters.ValidateLifetime = true;

            // This defines the maximum allowable clock skew - i.e. provides a tolerance on the token expiry time 
            // when validating the lifetime. As we're creating the tokens locally and validating them on the same 
            // machines which should have synchronised time, this can be set to zero. Where external tokens are
            // used, some leeway here could be useful.
            options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(0);

            // Use JWT Bearer authentication
            app.UseJwtBearerAuthentication(options);

            return app;
        }

    }
}
