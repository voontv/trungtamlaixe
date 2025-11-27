using Microsoft.IdentityModel.Tokens;
using Ttlaixe.AutoConfig;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ttlaixe.Providers
{
    [ImplementBy(typeof(TokenGenerator))]
    public interface ITokenGenerator
    {
        TokenResponse GenerateToken(string username);
    }

    public class TokenGenerator : ITokenGenerator
    {
        private readonly SecuritySettings securitySettings;

        public readonly SigningCredentials SigningCredentials;

        public TokenGenerator(SecuritySettings securitySettings)
        {
            this.securitySettings = securitySettings;

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(this.securitySettings.Secret));
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        }

        private static Claim CreateClaim(string type, string value)
        {
            return new Claim(type, value, null, ClaimsIdentity.DefaultIssuer, "Provider");
        }

        public TokenResponse GenerateToken(string username)
        {
            var claims = new List<Claim>
            {
                CreateClaim(ClaimTypes.NameIdentifier, username),
                CreateClaim(ClaimTypes.Name, username)
            };

            var identity = new ClaimsIdentity(claims, "Bearer");

            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                claims: identity.Claims,
                notBefore: now,
                expires: now.AddSeconds(securitySettings.Expiration),
                signingCredentials: SigningCredentials);

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new TokenResponse(token);
        }
    }
}