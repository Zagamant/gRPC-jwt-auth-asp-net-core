using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Server.gRPC.Helpers
{
    public class JwtTokenValidator : ISecurityTokenValidator
    {
        public bool CanReadToken(string securityToken) => true;

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "your string",
                ValidAudience = "your string",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your secrete code"))
            };

            var claimsPrincipal = handler.ValidateToken(securityToken, tokenValidationParameters, out validatedToken);
            return claimsPrincipal;
        }

        public bool CanValidateToken { get; } = true;
        public int MaximumTokenSizeInBytes { get; set; } = int.MaxValue;
    }
}
