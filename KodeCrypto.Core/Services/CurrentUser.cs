using KodeCrypto.Application.Common.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace KodeCrypto.Core.Services
{
    public class CurrentUser : IUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? Id
        {
            get
            {

                var context = _httpContextAccessor.HttpContext;
                if (context is null)
                {
                    return null;
                }
                var jwt = context.Request.Headers["Authorization"];
                var claims = ExtraxtClaims(jwt);
                return claims.Where(c => c.Type == "id")
                                        .Select(c => c.Value)
                                        .FirstOrDefault();
            }
            set { }
        }

        public IEnumerable<Claim> ExtraxtClaims(string jwt)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken securityToken = tokenHandler.ReadJwtToken(jwt);
            return securityToken.Claims;
        }
    }
}
