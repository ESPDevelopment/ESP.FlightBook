using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;

namespace ESP.FlightBook.Identity.Token
{
    public class RSAKeyUtils
    {
        /// <summary>
        /// Generates a random set of RSA parameters
        /// </summary>
        /// <returns>A unique set of RSA parameters</returns>
        public static RSAParameters GetRandomKey()
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    return rsa.ExportParameters(true);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        /// <summary>
        /// Creates a new random set of RSA key parameters and returns the result as a
        /// Base64-encoded string
        /// </summary>
        /// <returns>A Base64-encoded string representing random RSA key parameters.</returns>
        public static string GetEncodedKeyParameters()
        {
            // Generate security key and convert to Base64 encoded string
            RSAParametersWithPrivate rsaParameters = new RSAParametersWithPrivate();
            rsaParameters.SetParameters(GetRandomKey());
            string rsaParametersAsString = JsonConvert.SerializeObject(rsaParameters);
            byte[] rsaParametersAsByteArray = System.Text.Encoding.UTF8.GetBytes(rsaParametersAsString);
            string rsaParametersAsBase64String = Convert.ToBase64String(rsaParametersAsByteArray);
            return rsaParametersAsBase64String;
        }

        /// <summary>
        /// Creates an RSA security key from a Base64-encoded set of RSA key parameters.
        /// </summary>
        /// <param name="encodedKey">A Base64-encoded set of RSA key parameters.</param>
        /// <returns>An RSA security key based on the decoded RSA key parameters</returns>
        public static RsaSecurityKey GetDecodedKey(string encodedKey)
        {
            byte[] rsaParametersAsByteArray = Convert.FromBase64String(encodedKey);
            string rsaParametersAsString = System.Text.Encoding.UTF8.GetString(rsaParametersAsByteArray);
            RSAParametersWithPrivate rsaParametersWithPrivate = JsonConvert.DeserializeObject<RSAParametersWithPrivate>(rsaParametersAsString);
            RSAParameters rsaParameters = rsaParametersWithPrivate.ToRSAParameters();
            RsaSecurityKey securityKey = new RsaSecurityKey(rsaParametersWithPrivate.ToRSAParameters());
            return securityKey;
        }

        /// <summary>
        /// Utility class to allow restoring RSA parameters from JSON as the normal
        /// RSA parameters class won't restore private key info.
        /// </summary>
        private class RSAParametersWithPrivate
        {
            public byte[] D { get; set; }
            public byte[] DP { get; set; }
            public byte[] DQ { get; set; }
            public byte[] Exponent { get; set; }
            public byte[] InverseQ { get; set; }
            public byte[] Modulus { get; set; }
            public byte[] P { get; set; }
            public byte[] Q { get; set; }

            public void SetParameters(RSAParameters p)
            {
                D = p.D;
                DP = p.DP;
                DQ = p.DQ;
                Exponent = p.Exponent;
                InverseQ = p.InverseQ;
                Modulus = p.Modulus;
                P = p.P;
                Q = p.Q;
            }
            public RSAParameters ToRSAParameters()
            {
                return new RSAParameters()
                {
                    D = this.D,
                    DP = this.DP,
                    DQ = this.DQ,
                    Exponent = this.Exponent,
                    InverseQ = this.InverseQ,
                    Modulus = this.Modulus,
                    P = this.P,
                    Q = this.Q

                };
            }
        }
    }
}
