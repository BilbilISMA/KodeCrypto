using KodeCrypto.Application.Common.Interfaces;
using KodeCrypto.Domain.Entities.Identity;
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
                var jwt = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];

                var claims = ExtraxtClaims(jwt);
                return claims.Where(c => c.Type == "id")
                                        .Select(c => c.Value)
                                        .FirstOrDefault();
            }
        }

        public IEnumerable<Claim> ExtraxtClaims(string jwt)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken securityToken = tokenHandler.ReadJwtToken(jwt);
            return securityToken.Claims;
        }
    }
}
