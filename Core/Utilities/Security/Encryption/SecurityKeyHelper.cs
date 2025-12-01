using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Core.Utilities.Security.Encryption
{
    public static class SecurityKeyHelper
    {
        /// <summary>
        /// Creates a symmetric security key from the given string.
        /// </summary>
        public static SecurityKey CreateSecurityKey(string securityKey)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        }
    }
}
