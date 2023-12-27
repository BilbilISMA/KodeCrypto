using System;
using KodeCrypto.Application.Common.Interfaces;
using KodeCrypto.Domain.Entities;
using KodeCrypto.Domain.Enums;

namespace KodeCrypto.Infrastructure.Repositories
{
	public class ApiKeyRepository : IApiKeyRepository
    {
        private readonly IApplicationDbContext _dbContext;

        public ApiKeyRepository(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> SaveApiKey(ApiKey apiKey, CancellationToken cancellationToken)
        {
            try
            {
                await _dbContext.ApiKeys.AddAsync(apiKey);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UpdateApiKey(ApiKey apiKey, CancellationToken cancellationToken)
        {
            try
            {
                _dbContext.ApiKeys.Update(apiKey);
                await _dbContext.SaveChangesAsync(cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<ApiKey>> GetApiKeysByProviderId(ProviderEnum provider)
        {
            try
            {
                return _dbContext.ApiKeys.Where(x => x.ProviderId == provider).ToList();   
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ApiKey> GetApiKeyPerUser(string userId, ProviderEnum provider)
        {
            try
            {
                return _dbContext.ApiKeys.FirstOrDefault(x => x.UserId == userId && x.ProviderId == provider);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

