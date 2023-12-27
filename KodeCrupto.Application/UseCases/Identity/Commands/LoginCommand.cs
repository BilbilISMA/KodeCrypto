using FluentValidation;
using KodeCrypto.Application.Common.Interfaces;
using KodeCrypto.Application.Common.Options;
using KodeCrypto.Application.Generic;
using KodeCrypto.Application.UseCases.Identity.DTO;
using KodeCrypto.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KodeCrypto.Application.UseCases.Identity.Commands
{
    #region Command
    public class LoginCommand : IRequest<JsonWebTokenDTO>
    {

        public required string Email { get; set; }
        public required string Password { get; set; }
    }
    #endregion

    #region Handler
    public class LoginHandler : BaseHandlerRequest<LoginCommand, JsonWebTokenDTO>
    {
        private readonly UserManager<User> _userManager;
        private readonly IOptions<JwtOptions> _jwtOptions;
        private readonly IIdentityService _identityService;

        public LoginHandler(BaseHandlerServices services, UserManager<User> userManager, IOptions<JwtOptions> jwtOptions, IIdentityService identityService) : base(services)
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions;
            _identityService = identityService;
        }

        public override async Task<JsonWebTokenDTO> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new Exception("Authentification failed. Make sure you provide the correct email.");
            }

            bool validPassword = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!validPassword)
            {
                throw new Exception("Authentification failed. Make sure you provide the correct password.");
            }

            string accessToken = _identityService.GenerateAccessToken(user.Id);
            string refreshToken = _identityService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(_jwtOptions.Value.ExpiryMinutesRefreshToken);
            try
            {
                IdentityResult updateResult = await _userManager.UpdateAsync(user);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unexpected error while saving changes in database");
                throw;
            }
            return new JsonWebTokenDTO() { RefreshToken = refreshToken, AccessToken = accessToken };
        }
    }
    #endregion

    #region Validation
    public class LoginValidator : AbstractValidator<LoginCommand>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
    #endregion
}

