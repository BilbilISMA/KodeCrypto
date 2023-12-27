using KodeCrypto.Domain.Entities;
using KodeCrypto.Domain.Enums;

namespace KodeCrypto.Application.Common.Interfaces
{
    public interface IApiKeyRepository
	{
        Task<List<ApiKey>> GetApiKeysByProviderId(ProviderEnum provider);
        Task<ApiKey> GetApiKeyPerUser(string userId, ProviderEnum provider);
        Task<bool> SaveApiKey(ApiKey apiKey, CancellationToken cancellationToken);
        Task<bool> UpdateApiKey(ApiKey apiKey, CancellationToken cancellationToken);
    }
}

