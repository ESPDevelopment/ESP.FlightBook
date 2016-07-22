using System.Collections.Generic;
using System.Security.Claims;

namespace ESP.FlightBook.Tests.Api.Controllers
{
    public class ControllerUnitTests
    {
        /// <summary>
        /// Constructs a mock identity with the necessary nameidentifier claim
        /// </summary>
        /// <param name="userId">Unique registered user id</param>
        /// <returns>A claims principal that can be used with controller functions</returns>
        protected ClaimsPrincipal GetUser(string userId)
        {
            // Create necessary claims
            Claim userIdClaim = new Claim(ClaimTypes.NameIdentifier, userId, ClaimValueTypes.String, "http://espdevelopment.com");
            List<Claim> claims = new List<Claim>();
            claims.Add(userIdClaim);

            // Create principal
            ClaimsIdentity identity = new ClaimsIdentity(claims);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            return principal;
        }
    }
}
