using KodeCrypto.Application.Common.Interfaces;
using KodeCrypto.Application.Generic;
using KodeCrypto.Domain.Entities;
using KodeCrypto.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

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

        public SaveApiKeyHandler(BaseHandlerServices services, IApiKeyRepository apiKeyRepository) : base(services)
        {
            _apiKeyRepository = apiKeyRepository;
        }

        public override async Task<bool> Handle(SaveApiKeyCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var existingApiKey = await _apiKeyRepository.GetApiKeyPerUser(_user.Id ?? string.Empty, command.ProviderId);
                if (existingApiKey == null)
                {
                    var apiKey = new ApiKey
                    {
                        UserId = _user.Id ?? string.Empty,
                        Key = command.ApiKey,
                        Secret = command.Secret,
                        ProviderId = command.ProviderId
                    };

                    await _apiKeyRepository.SaveApiKey(apiKey, cancellationToken);
                }
                else
                {
                    existingApiKey.Key = command.ApiKey;
                    existingApiKey.Secret = command.Secret;

                    await _apiKeyRepository.UpdateApiKey(existingApiKey, cancellationToken);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error happened during {action} with {@request} and {message} : ", nameof(SaveApiKeyCommand), command, ex);
                throw;
            }
        }
    }

}

