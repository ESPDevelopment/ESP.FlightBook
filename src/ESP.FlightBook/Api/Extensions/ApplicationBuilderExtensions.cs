using Microsoft.AspNetCore.Builder;

namespace ESP.FlightBook.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseESPCors(this IApplicationBuilder app)
        {
            // Enable cross-origin requests
            string[] exposedHeaders = {
                    "X-eFlightBook-Pagination-Total",
                    "X-eFlightBook-Pagination-Limit",
                    "X-eFlightBook-Pagination-Page",
                    "X-eFlightBook-Pagination-Returned",
                    "X-eFlightBook-Pagination-TotalPages"
                };
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders(exposedHeaders));

            return app;
        }
    }
}
