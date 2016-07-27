using Microsoft.AspNetCore.Builder;

namespace ESP.FlightBook.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseESPCors(this IApplicationBuilder app)
        {
            // Define exposed headers
            string[] exposedHeaders = {
                    "X-eFlightBook-Pagination-Total",
                    "X-eFlightBook-Pagination-Limit",
                    "X-eFlightBook-Pagination-Page",
                    "X-eFlightBook-Pagination-Returned",
                    "X-eFlightBook-Pagination-TotalPages"
                };

            // Define allowed origins
            string[] allowedOrigins =
            {
                "https://eflightbook.com",
                "https://www.eflightbook.com",
                "https://esp-flightbook.azurewebsites.net"
            };

            // Enable cross-origin requests
            app.UseCors(builder => builder
                //.AllowAnyOrigin()
                .WithOrigins(allowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders(exposedHeaders));

            return app;
        }
    }
}
