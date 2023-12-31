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

namespace KodeCrypto.Infrastructure.Integration.Binance
{
    public class BinanceWebhook : IWebhook
    {
        private readonly IMapper _mapper;
        private readonly IUser _user;
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly BinanceApiClient _apiClient;
        private readonly IOptions<BinanceConfig> _config;
        private readonly ILogger<BinanceWebhook> _logger;

        public BinanceWebhook(IMapper mapper,
            IUser user,
            IApiKeyRepository apiKeyRepository,
            BinanceApiClient apiClient,
            IOptions<BinanceConfig> config,
            ILogger<BinanceWebhook> logger
            )
        {
            _mapper = mapper;
            _user = user;
            _apiKeyRepository = apiKeyRepository;
            _apiClient = apiClient;
            _config = config;
            _logger = logger;
        }

        public async Task<bool> ProcessOrderAsync(Order order)
        {            
            try
            {
                var orderRequest = _mapper.Map<BinanceOrderRequest>(order);
                var key = await _apiKeyRepository.GetApiKeyPerUser(_user.Id ?? string.Empty, ProviderEnum.Binance);
                string jsonBody = JsonConvert.SerializeObject(orderRequest);

                // Create StringContent with JSON body
                StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                _logger.LogInformation("Posting Order with {@Content} using {Key}", content, key);

                return await _apiClient.PostRequestAsync(_config.Value.OrderEndpoint, content, key);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error happened during {action} with {@request} and {message} : ", nameof(ProcessOrderAsync), order, ex);
                throw;
            }
        }
    }
}

