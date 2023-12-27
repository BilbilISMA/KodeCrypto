using FluentValidation;
using KodeCrypto.Application.Generic;
using KodeCrypto.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace KodeCrypto.Application.UseCases.Identity.Commands
{
    public class SignUpCommand : IRequest<bool>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class SignUpHandler : BaseHandlerRequest<SignUpCommand, bool>
    {
        private readonly UserManager<User> _userManager;

        public SignUpHandler(BaseHandlerServices services, UserManager<User> userManager) : base(services)
        {
            _userManager = userManager;
        }

        public override async Task<bool> Handle(SignUpCommand request, CancellationToken cancellationToken)
        {
            User? user = await _userManager.FindByEmailAsync(request.Email);

            // 1. If the e-mail is already saved in the database, block the process and show a message “E-mail is already in use"
            if (user != null)
            {
                throw new Exception("Account associated with this email already exists.");
            }

            // 2. map data
            User newUser = new();
            newUser.FirstName = request.FirstName;
            newUser.LastName = request.LastName;
            newUser.Email = request.Email;
            newUser.UserName = request.Email;
            newUser.Token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

            try
            {
                // 3. Create the user using UserManager
                IdentityResult createUserResult = await _userManager.CreateAsync(newUser, request.Password);

                if (!createUserResult.Succeeded)
                {
                    throw new Exception(string.Join(",", createUserResult.Errors.Select(x => x.Description).ToArray()));
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "User registration failed.");
                throw;
            }

            return true;
        }

        public class SignUpValidator : AbstractValidator<SignUpCommand>
        {
            public SignUpValidator()
            {
                RuleFor(x => x.Email).EmailAddress();
                RuleFor(x => x.FirstName).NotEmpty().NotNull();
                RuleFor(x => x.LastName).NotEmpty().NotNull();
                RuleFor(x => x.Password).NotEmpty().NotNull();
            }
        }
    }
}