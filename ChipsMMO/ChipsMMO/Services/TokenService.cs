using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace ChipsMMO.Services
{
    public class TokenService
    {
        public TokenService() { }

        public string GenerateJSONAccessToken(string username)
        {
            var securityKey = Convert.FromBase64String(Environment.GetEnvironmentVariable("CHIPSMMO_ACCESS_TOKEN_SECRET"));
            var signingCredentials = new SigningCredentials(

            new SymmetricSecurityKey(securityKey),
            SecurityAlgorithms.HmacSha256,
            SecurityAlgorithms.Sha256);
            var nbf = DateTime.UtcNow.AddYears(-1); ;
            var exp = DateTime.UtcNow.AddMinutes(120);
            var payload = new JwtPayload("chipsmmo.cc", "chipsmmo.cc", new[] { new Claim(JwtRegisteredClaimNames.Sub, username) }, nbf, exp);
            var jwtToken = new JwtSecurityToken(new JwtHeader(signingCredentials), payload);
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            return jwtTokenHandler.WriteToken(jwtToken);
        }

        public string GenerateJSONRefreshToken(string username)
        {
            var securityKey = Convert.FromBase64String(Environment.GetEnvironmentVariable("CHIPSMMO_REFRESH_TOKEN_SECRET"));
            var signingCredentials = new SigningCredentials(

            new SymmetricSecurityKey(securityKey),
            SecurityAlgorithms.HmacSha256,
            SecurityAlgorithms.Sha256);
            var nbf = DateTime.UtcNow.AddYears(-1);
            var exp = DateTime.UtcNow.AddYears(1);
            var payload = new JwtPayload("chipsmmo.cc", "chipsmmo.cc", new[] { new Claim(JwtRegisteredClaimNames.Sub, username) }, nbf, exp);
            var jwtToken = new JwtSecurityToken(new JwtHeader(signingCredentials), payload);
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            return jwtTokenHandler.WriteToken(jwtToken);
        }

        public string GetUserNameFromRefreshToken(string refreshToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(refreshToken);

            return jsonToken.Claims.First(claim => claim.Type == "sub").Value;
        }

        public string GetUserNameFromBearerToken(string bearerToken)
        {
            var accessToken = bearerToken.Replace("Bearer ", string.Empty);
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(accessToken);

            return jsonToken.Claims.First(claim => claim.Type == "sub").Value;
        }

        public bool IsRefreshTokenValid(string refreshToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = "chipsmmo.cc",
                    ValidAudience = "chipsmmo.cc",
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(Environment.GetEnvironmentVariable("CHIPSMMO_REFRESH_TOKEN_SECRET")))
                }, out _);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
