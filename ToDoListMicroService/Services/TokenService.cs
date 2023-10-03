using System.Security.Claims;

namespace ToDoListMicroService.Services
{
    public class TokenService : ITokenService
    {
        #region Private declaration
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Public Methods
        public TokenService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetName()
        {

            var result = string.Empty;

            if (_httpContextAccessor.HttpContext != null)
            {
                var claims = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                
                if (claims != null)
                {
                    result = claims.ToString();
                }
            }

            return result;
        }
        #endregion
    }
}
