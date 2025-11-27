using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Ttlaixe.Providers;
using System;
using System.Text;

namespace Ttlaixe.LibsStartup
{
    public static class SecurityConfig
    {
        private static void SetJwtOption(JwtBearerOptions options, byte[] key)
        {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };
            options.SaveToken = true;
        }
        public static void ConfigSecurity(this IServiceCollection services, SecuritySettings securitySettings)
        {
            var key = Encoding.ASCII.GetBytes(securitySettings.Secret);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => SetJwtOption(options, key));
        }
    }
}