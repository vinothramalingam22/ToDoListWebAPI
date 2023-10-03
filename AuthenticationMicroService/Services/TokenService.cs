using AuthenticationMicroService.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;

namespace AuthenticationMicroService.Services
{
    public class TokenService : ITokenService
    {
        #region Private declaration
        
        private readonly IConfiguration _configuration;

        #endregion

        public TokenService(IConfiguration configuration)
        {            
            _configuration = configuration;
        }

        #region Public Methods
        public string GenerateToken(Login userInfo)
        {                  
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                 _configuration.GetSection("AppSettings:Token").Value));

            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name, userInfo.UserName),
                    new Claim(JwtRegisteredClaimNames.Sub, userInfo.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, userInfo.RoleName)
                }),
                Expires = DateTime.Now.AddMinutes(20),
                Issuer = _configuration.GetSection("Jwt:Issuer").Value,
                Audience = _configuration.GetSection("Jwt:Audience").Value,
                SigningCredentials = signinCredentials,
                IssuedAt = DateTime.UtcNow
            };

            var tokenHandler = new JwtSecurityTokenHandler();            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;
        }

        #endregion
    }
}
