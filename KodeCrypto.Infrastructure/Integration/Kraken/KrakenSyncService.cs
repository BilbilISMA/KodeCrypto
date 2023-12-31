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

namespace KodeCrypto.Infrastructure.Integration.Kraken
{
    public class KrakenSyncService : ISyncService
    {
        private readonly KrakenApiClient _apiClient;
        private readonly IMapper _mapper;
        private readonly ILocalDataRepository _localDataRepository;
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly IUser _user;
        private readonly IOptions<KrakenConfig> _config;
        private readonly ILogger<KrakenSyncService> _logger;

        private long _nonce;

        public KrakenSyncService(KrakenApiClient apiClient, IMapper mapper, ILocalDataRepository localDataRepository, IApiKeyRepository apiKeyRepository, IUser user, IOptions<KrakenConfig> config , ILogger<KrakenSyncService> logger)
        {
            _apiClient = apiClient;
            _mapper = mapper;
            _localDataRepository = localDataRepository;
            _apiKeyRepository = apiKeyRepository;
            _user = user;
            _nonce = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); // Initial nonce value
            _config = config;
            _logger = logger;
        }

        public async Task<bool> SyncAll()
        {
            try
            {
                // Get the list of methods with the Sync attribute
                List<MethodInfo> syncMethods = typeof(KrakenSyncService).GetMethodsWithAttribute(typeof(SyncAttribute));

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
                var apiKeys = await _apiKeyRepository.GetApiKeysByProviderId(ProviderEnum.Kraken);
                foreach (var key in apiKeys)
                {
                    var nonce = GetNonce();
                    var response = await _apiClient.PostRequestAsync(_config.Value.BalanceEndpoint, new StringContent(string.Empty), key, nonce);

                    // Parse the response and return the balance
                    var parsedData = JsonConvert.DeserializeObject<KrakenBalanceResponse>(response);
                    var accountBalance = _mapper.Map<AccountBalance>(parsedData);
                    accountBalance.ProviderId = ProviderEnum.Kraken;
                    accountBalance.UserId = key.UserId;
                    await _localDataRepository.SaveAccountBalance(accountBalance, CancellationToken.None);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error happened during {action} with {message} : ", nameof(SyncBalance), ex);
                throw;
            }
           
        }

        [Sync]
        public async Task<bool> SyncTransactionHistory()
        {
            try
            {
                var apiKeys = await _apiKeyRepository.GetApiKeysByProviderId(ProviderEnum.Kraken);
                foreach (var key in apiKeys)
                {
                    var nonce = GetNonce();
                    string jsonBody = JsonConvert.SerializeObject(new { nonce, asset = "xdg" });

                    // Create StringContent with JSON body
                    StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    var response = await _apiClient.PostRequestAsync(_config.Value.TradeBalanceEndpoint, content, key, nonce);

                    // Parse the response and return the transaction history
                    var parsedData = JsonConvert.DeserializeObject<KrakenTradeHistoryResponse>(response);
                    var tradeHistories = _mapper.Map<List<TradeHistory>>(parsedData?.Result.Trades);
                    tradeHistories.ForEach(x => x.UserId = key.UserId);
                    await _localDataRepository.SaveTradeHistories(tradeHistories, CancellationToken.None);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error happened during {action} with {message} : ", nameof(SyncTransactionHistory), ex);
                throw;
            }
           
        }

        [Sync]
        public async Task<bool> SyncOrders()
        {
            try
            {
                var ordersToSync = await _localDataRepository.GetOrdersToSync();
                var apiKeys = await _apiKeyRepository.GetApiKeysByProviderId(ProviderEnum.Kraken);
                foreach (var key in apiKeys)
                {
                    foreach (var order in ordersToSync.Where(x => x.UserId == key.UserId)) 
                    {
                        var orderRequest = _mapper.Map<KrakenOrderRequest>(order);
                        string jsonBody = JsonConvert.SerializeObject(orderRequest);

                        // Create StringContent with JSON body
                        StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                        var nonce = GetNonce();
                        await _apiClient.PostRequestAsync(_config.Value.OrderEndpoint, content, key, nonce);
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

        private long GetNonce()
        {
            return Interlocked.Increment(ref _nonce);
        }
    }
}