using Microsoft.IdentityModel.Tokens;

namespace Core.Utilities.Security.Encryption
{
    public static class SigningCredentialsHelper
    {
        /// <summary>
        /// Creates signing credentials using HmacSha512.
        /// </summary>
        public static SigningCredentials CreateSigningCredentials(SecurityKey securityKey)
        {
            return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);
        }
    }
}
