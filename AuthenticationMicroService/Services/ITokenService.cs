using AuthenticationMicroService.Models;

namespace AuthenticationMicroService.Services
{
    public interface ITokenService
    {
        public string GenerateToken(Login userInfo);     
    }
}
