using System.Reflection;
using System.Text;
using AutoMapper;
using KodeCrypto.Application.Common.Attributes;
using KodeCrypto.Application.Common.Extensions;
using KodeCrypto.Application.Common.Interfaces;
using KodeCrypto.Application.DTO.Requests;
using KodeCrypto.Application.DTO.Responses;
using KodeCrypto.Domain.Entities;
using KodeCrypto.Domain.Enums;
using KodeCrypto.Infrastructure.Integration.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace KodeCrypto.Infrastructure.Integration.Binance
{
    public class BinanceSyncService : ISyncService
    {
        private readonly BinanceApiClient _apiClient;
        private readonly IMapper _mapper;
        private readonly ILocalDataRepository _localDataRepository;
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly IOptions<BinanceConfig> _config;
        private readonly ILogger<BinanceSyncService> _logger;

        private readonly IUser _user;
        public BinanceSyncService(BinanceApiClient apiClient, IMapper mapper, ILocalDataRepository localDataRepository, IApiKeyRepository apiKeyRepository, IUser user, IOptions<BinanceConfig> config, ILogger<BinanceSyncService> logger)
        {
            _apiClient = apiClient;
            _mapper = mapper;
            _localDataRepository = localDataRepository;
            _apiKeyRepository = apiKeyRepository;
            _user = user;
            _config = config;
            _logger = logger;
        }

        public async Task<bool> SyncAll()
        {
            try
            {
                // Get the list of methods with the Sync attribute
                List<MethodInfo> syncMethods = typeof(BinanceSyncService).GetMethodsWithAttribute(typeof(SyncAttribute));

                var tasks = syncMethods.Select(method => method.Invoke(this, null) as Task).ToArray();

                // Wait for all tasks to complete
                await Task.WhenAll(tasks);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error happened during {action} with {message} : ", nameof(SyncAll), ex);
                throw;
            }
        }

        [Sync]
        public async Task<bool> SyncBalance()
        {
            try
            {
                var apiKeys = await _apiKeyRepository.GetApiKeysByProviderId(ProviderEnum.Binance);
                foreach (var key in apiKeys)
                {
                    var response = await _apiClient.GetRequestAsync(_config.Value.BalanceEndpoint, string.Empty, key);

                    // Parse the response and return the balance
                    var parsedData = JsonConvert.DeserializeObject<BinanceBalanceResponse>(response);
                    var accountBalance = _mapper.Map<AccountBalance>(parsedData);
                    accountBalance.ProviderId = ProviderEnum.Binance;
                    accountBalance.UserId = key.UserId;
                    await _localDataRepository.SaveAccountBalance(accountBalance, CancellationToken.None);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error happened during {nameof(SyncBalance)} with message : ", ex);
                throw;
            }           
        }

        [Sync]
        public async Task<bool> SyncTransactionHistory()
        {
            try
            {
                var apiKeys = await _apiKeyRepository.GetApiKeysByProviderId(ProviderEnum.Binance);

                foreach (var key in apiKeys)
                {
                    var response = await _apiClient.GetRequestAsync(_config.Value.TradeBalanceEndpoint, string.Empty, key);
                
                    // Parse the response and return the transaction history
                    var parsedData = JsonConvert.DeserializeObject<BinanceTradeHistoryResponse>(response);
                    var tradeHistories = _mapper.Map<List<TradeHistory>>(parsedData?.Result);
                    tradeHistories.ForEach(x => { x.UserId = key.UserId; x.ProviderId = ProviderEnum.Binance; });

                    _logger.LogInformation("Saving trade with {@Content} using {Key}", tradeHistories, key);

                    await _localDataRepository.SaveTradeHistories(tradeHistories, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error happened during {nameof(SyncTransactionHistory)} with message : ", ex);
                throw;
            }
            return true;
        }

        [Sync]
        public async Task<bool> SyncOrders()
        {
            try
            {
                var ordersToSync = await _localDataRepository.GetOrdersToSync();
                var apiKeys = await _apiKeyRepository.GetApiKeysByProviderId(ProviderEnum.Binance);
                foreach (var key in apiKeys)
                {
                    foreach (var order in ordersToSync.Where(x => x.UserId == key.UserId))
                    {
                        var orderRequest = _mapper.Map<BinanceOrderRequest>(order);
                        string jsonBody = JsonConvert.SerializeObject(orderRequest);

                        // Create StringContent with JSON body
                        StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                        await _apiClient.PostRequestAsync(_config.Value.OrderEndpoint, content, key);
                    }
                }
                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError("An error happened during {action} with {message} : ", nameof(SyncOrders), ex);
                throw;
            }
        }
    }
}