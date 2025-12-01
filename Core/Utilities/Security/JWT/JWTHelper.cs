using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Core.Entities.Concrete;
using Core.Utilities.Security.Encryption;
using Core.Extensions;

namespace Core.Utilities.Security.JWT
{
    public class JWTHelper : ITokenHelper
    {
        public IConfiguration Configuration { get; }
        private readonly TokenOptions _tokenOptions;
        private DateTime _accessTokenExpiration;

        public JWTHelper(IConfiguration configuration)
        {
            Configuration = configuration;
            // TokenOptions, appsettings.json'daki "TokenOptions" bölümünden okunur.
            _tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>()
                           ?? throw new InvalidOperationException("TokenOptions configuration section is missing.");
        }

        /// <summary>
        /// Kullanıcı ve yetkiler için JWT access token üretir.
        /// </summary>
        public AccessToken CreateToken(User user, List<OperationClaim> operationClaims)
        {
            _accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);

            var securityKey       = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
            var signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);
            var jwt                = CreateJwtSecurityToken(_tokenOptions, user, signingCredentials, operationClaims);

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(jwt);

            return new AccessToken
            {
                Token      = token,
                Expiration = _accessTokenExpiration
            };
        }

        public JwtSecurityToken CreateJwtSecurityToken(
            TokenOptions tokenOptions,
            User user,
            SigningCredentials signingCredentials,
            List<OperationClaim> operationClaims)
        {
            var jwt = new JwtSecurityToken(
                issuer: tokenOptions.Issuer,
                audience: tokenOptions.Audience,
                expires: _accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: SetClaims(user, operationClaims),
                signingCredentials: signingCredentials
            );

            return jwt;
        }

        private IEnumerable<Claim> SetClaims(User user, List<OperationClaim> operationClaims)
        {
            var claims = new List<Claim>();

            // Core.Extensions.ClaimExtensions kullanımı:
            claims.AddNameIdentifier(user.Id.ToString());
            claims.AddEmail(user.Email);
            claims.AddName(user.UserName);

            // Ek olarak UserId'yi custom claim olarak da ekleyelim.
            claims.Add(new Claim("UserId", user.Id.ToString()));

            claims.AddRoles(operationClaims.Select(c => c.Name).ToArray());

            return claims;
        }
    }
}
