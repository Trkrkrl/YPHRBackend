using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Core.Extensions
{
    public static class ClaimExtensions
    {
        public static void AddEmail(this ICollection<Claim> claims, string? email)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Email, email));
            }
        }

        public static void AddName(this ICollection<Claim> claims, string? name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                claims.Add(new Claim(ClaimTypes.Name, name));
            }
        }

        public static void AddNameIdentifier(this ICollection<Claim> claims, string? nameIdentifier)
        {
            if (!string.IsNullOrWhiteSpace(nameIdentifier))
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, nameIdentifier));
            }
        }

        public static void AddRoles(this ICollection<Claim> claims, string[] roles)
        {
            if (roles == null || roles.Length == 0)
                return;

            foreach (var role in roles.Where(r => !string.IsNullOrWhiteSpace(r)))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }
    }
}
