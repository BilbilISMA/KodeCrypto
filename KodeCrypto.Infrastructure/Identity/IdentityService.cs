using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KodeCrypto.Application.Common.Interfaces;
using KodeCrypto.Application.Common.Models;
using KodeCrypto.Application.Common.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using KodeCrypto.Domain.Entities.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;

namespace KodeCrypto.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;
    private readonly IUserClaimsPrincipalFactory<User> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;
    private readonly IOptions<JwtOptions> _jwtOptions;

    public IdentityService(
        UserManager<User> userManager,
        IUserClaimsPrincipalFactory<User> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService,
        IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
        _jwtOptions = jwtOptions;
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

        return user.UserName;
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
    {
        var user = new User
        {
            UserName = userName,
            Email = userName,
        };

        var result = await _userManager.CreateAsync(user, password);

        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

        return user != null ? await DeleteUserAsync(user.Id) : Result.Success();
    }

    public async Task<Result> DeleteUserAsync(User user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }

    public string? GenerateAccessToken(string userId)
    {
        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(_jwtOptions.Value.SecretKey));
        SigningCredentials signCredentials = new(securityKey, SecurityAlgorithms.HmacSha256);

        var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);
        if (user is null)
        {
            return null;
        }

        var claims = new List<Claim>
        {
            new Claim("email", user.Email),
            new Claim("id", user.Id.ToString()),
            new Claim(ClaimTypes.Role, "Admin")
        };

         _userManager.AddClaimsAsync(user, claims);
        var appIdentity = new ClaimsIdentity(claims);

        DateTime nowUtc = DateTime.UtcNow;
        DateTime expires = nowUtc.AddMinutes(_jwtOptions.Value.ExpiryMinutesAccessToken);

        // Create the JWT and write it to a string
        JwtSecurityToken token = new(
                               claims: claims,
                               expires: expires,
                               signingCredentials: signCredentials
                               );
        string jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    public string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
