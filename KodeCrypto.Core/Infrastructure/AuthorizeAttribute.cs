using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using KodeCrypto.Application.Common.Interfaces;
using KodeCrypto.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Infrastructure;

public class AuthorizeAttribute : TypeFilterAttribute
{
    public AuthorizeAttribute(params string[] permissions) : base(typeof(ClaimsRequirementFilter)) => Arguments = new object[] { ClaimsPermissionType.All, permissions };

    public AuthorizeAttribute(ClaimsPermissionType permissionType, string[] permissions) : base(typeof(ClaimsRequirementFilter)) => Arguments = new object[] { permissionType, permissions };

    private class ClaimsRequirementFilter : IAuthorizationFilter
    {
        private readonly string[] _role;
        private readonly ClaimsPermissionType _permissionType;
        private readonly IApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public ClaimsRequirementFilter(ClaimsPermissionType permissionType, IApplicationDbContext dbContext, string[] permissions, UserManager<User> userManager)
        {
            if (permissions.Length == 0)
            {
                throw new ArgumentException(nameof(permissions));
            }

            _role = permissions;
            _permissionType = permissionType;
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            var jwt = context.HttpContext.Request.Headers["Authorization"];

            var identity = user.Identity as ClaimsIdentity;
            var claims = ExtraxtClaims(jwt);
            var userId = claims.Where(c => c.Type == "id")
                                        .Select(c => c.Value)
                                        .FirstOrDefault();

            if (user.Identities.Count() == 0 || userId == null || !claims.Any(x => x.Type == ClaimTypes.Role))
            {
                context.Result = new ForbidResult();
                return;
            }

            var role = claims.Where(c => c.Type == ClaimTypes.Role)
                                       .Select(c => c.Value)
                                       .ToList();

            if (!role.Intersect(_role).Any())
            {
                context.Result = new ForbidResult();
            }
        }

        public static int? StringAsNullableInt(string input) => int.TryParse(input, out var f) ? f : default(int?);

        public IEnumerable<Claim> ExtraxtClaims(string jwt)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken securityToken = (JwtSecurityToken)tokenHandler.ReadJwtToken(jwt);
            return securityToken.Claims;
        }
    } 
}