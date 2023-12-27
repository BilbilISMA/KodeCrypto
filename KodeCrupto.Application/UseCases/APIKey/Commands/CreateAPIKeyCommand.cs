using KodeCrypto.Application.Common.Interfaces;
using KodeCrypto.Application.Generic;
using KodeCrypto.Domain.Entities;
using KodeCrypto.Domain.Enums;
using MediatR;

namespace KodeCrypto.Application.UseCases.APIKey.Commands
{
    public class SaveApiKeyCommand : IRequest<bool>
    {
        public required string ApiKey { get; set; }
        public required string Secret { get; set; }
        public ProviderEnum ProviderId { get; set; }
    }

    public class SaveApiKeyHandler : BaseHandlerRequest<SaveApiKeyCommand, bool>
    {
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly IUser _user;

        public SaveApiKeyHandler(BaseHandlerServices services, IApiKeyRepository apiKeyRepository, IUser user) : base(services)
        {
            _apiKeyRepository = apiKeyRepository;
            _user = user;
        }

        public override async Task<bool> Handle(SaveApiKeyCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var existingApiKey = await _apiKeyRepository.GetApiKeyPerUser("488ba4c5-cb2a-4e82-acba-8eff7fcf1621", command.ProviderId);
                if (existingApiKey == null)
                {
                    var apiKey = new ApiKey
                    {
                        UserId = "488ba4c5-cb2a-4e82-acba-8eff7fcf1621",
                        Key = command.ApiKey,
                        Secret = command.Secret,
                        ProviderId = command.ProviderId
                    };

                    await _apiKeyRepository.SaveApiKey(apiKey, cancellationToken);
                }
                else
                {
                    existingApiKey.Key = command.ApiKey;
                    existingApiKey.Secret= command.Secret;
                    _apiKeyRepository.UpdateApiKey(existingApiKey, cancellationToken);
                }
                return true;
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw;
            }
        }
    }

}

