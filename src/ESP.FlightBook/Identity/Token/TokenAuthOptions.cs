using Microsoft.IdentityModel.Tokens;

namespace ESP.FlightBook.Identity.Token
{
    public class TokenAuthOptions
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public SigningCredentials SigningCredentials { get; set; }
        public SecurityKey SigningKey { get; set; }
    }
}
