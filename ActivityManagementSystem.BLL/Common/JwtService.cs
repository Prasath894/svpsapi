using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ActivityManagementSystem.BLL.Common
{
    /// <summary>
    /// Service class for generating jwt token
    /// </summary>
    public class JwtService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtService(string secretKey, string issuer, string audience)
        {
            _secretKey = secretKey;
            _issuer = issuer;
            _audience = audience;
        }

        /// <summary>
        /// To generate token based on the user claims.
        /// </summary>
        /// <param name="claims">The list of claims for the user.</param>
        /// <param name="expires">The token expiry time.</param>
        /// <returns></returns>
        public string GenerateToken(IEnumerable<Claim> claims, DateTime expires)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                claims: claims,
                expires: expires,
                audience: _audience,
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}
