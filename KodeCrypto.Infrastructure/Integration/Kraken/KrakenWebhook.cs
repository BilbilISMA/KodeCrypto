using System.Text;
using AutoMapper;
using KodeCrypto.Application.Common.Interfaces;
using KodeCrypto.Application.DTO.Requests;
using KodeCrypto.Domain.Entities;
using KodeCrypto.Domain.Enums;
using KodeCrypto.Infrastructure.Integration.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace KodeCrypto.Infrastructure.Integration.Kraken
{
    public class KrakenWebhook : IWebhook
    {
        private readonly IMapper _mapper;
        private readonly IUser _user;
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly KrakenApiClient _apiClient;
        private readonly IOptions<KrakenConfig> _config;
        private readonly ILogger<KrakenWebhook> _logger;

        private long _nonce;

        public KrakenWebhook(IMapper mapper,
            IUser user,
            IApiKeyRepository apiKeyRepository,
            KrakenApiClient client,
            IOptions<KrakenConfig> config,
            ILogger<KrakenWebhook> logger
            )
        {
            _mapper = mapper;
            _user = user;
            _apiKeyRepository = apiKeyRepository;
            _apiClient = client;
            _config = config;
            _logger = logger;
        }

        public async Task<bool> ProcessOrderAsync(Order order)
        {
            try
            {
                var orderRequest = _mapper.Map<KrakenOrderRequest>(order);

                var key = await _apiKeyRepository.GetApiKeyPerUser(_user.Id ?? string.Empty, ProviderEnum.Kraken);
                string jsonBody = JsonConvert.SerializeObject(orderRequest);

                // Create StringContent with JSON body
                StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var nonce = GetNonce();
                await _apiClient.PostRequestAsync(_config.Value.OrderEndpoint, content, key, nonce);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error happened during {action} with {@request} and {message} : ", nameof(ProcessOrderAsync), order, ex);
                throw;
            }
        }

        private long GetNonce()
        {
            return Interlocked.Increment(ref _nonce);
        }
    }
}

