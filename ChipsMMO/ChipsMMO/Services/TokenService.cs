using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChipsMMO.Services
{
    public class TokenService
    {
        public TokenService() { }

        public string GenerateJSONAccessToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Utility.StringToByteArray(Environment.GetEnvironmentVariable("CHIPSMMO_ACCESS_TOKEN_SECRET")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken("chipsmmo.cc",
              "chipsmmo.cc",
              new[] { new Claim(JwtRegisteredClaimNames.Sub, username) },
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            token.Payload.Add("iat", ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds());

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateJSONRefreshToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Utility.StringToByteArray(Environment.GetEnvironmentVariable("CHIPSMMO_REFRESH_TOKEN_SECRET")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken("chipsmmo.cc",
              "chipsmmo.cc",
              new[] { new Claim(JwtRegisteredClaimNames.Sub, username) },
              expires: DateTime.Now.AddYears(1),
              signingCredentials: credentials);

            token.Payload.Add("iat", ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds());

            return new JwtSecurityTokenHandler().WriteToken(token);
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
                    IssuerSigningKey = new SymmetricSecurityKey(Utility.StringToByteArray(Environment.GetEnvironmentVariable("CHIPSMMO_REFRESH_TOKEN_SECRET")))
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
