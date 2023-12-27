using System.Text;
using AutoMapper;
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
    public class KrakenService : IKrakenService 
	{
        private readonly KrakenApiClient _krakenApiClient;
        private readonly IMapper _mapper;
        private readonly ILocalDataRepository _localDataRepository;
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly IUser _user;
        private readonly IOptions<KrakenConfig> _krakenOptions;
        private readonly ILogger<KrakenService> _logger;

        private long _nonce;

        public KrakenService(KrakenApiClient krakenApiClient, IMapper mapper, ILocalDataRepository localDataRepository, IApiKeyRepository apiKeyRepository, IUser user, IOptions<KrakenConfig> krakenOptions , ILogger<KrakenService> logger)
        {
            _krakenApiClient = krakenApiClient;
            _mapper = mapper;
            _localDataRepository = localDataRepository;
            _apiKeyRepository = apiKeyRepository;
            _user = user;
            _nonce = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); // Initial nonce value
            _krakenOptions = krakenOptions;
            _logger = logger;
        }

        public async Task<bool> GetBalanceAsync()
        {
            try
            {
                var apiKeys = await _apiKeyRepository.GetApiKeysByProviderId(ProviderEnum.Kraken);
                foreach (var key in apiKeys)
                {
                    var nonce = GetNonce();
                    var response = await _krakenApiClient.PostRequestAsync(_krakenOptions.Value.BalanceEndpoint, new StringContent(string.Empty), key, nonce);

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
                _logger.LogError("An error happened during {action} with {message} : ", nameof(GetBalanceAsync), ex);
                throw;
            }
           
        }

        public async Task<bool> GetTransactionHistoryAsync()
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

                    var response = await _krakenApiClient.PostRequestAsync(_krakenOptions.Value.TradeBalanceEndpoint, content, key, nonce);

                    // Parse the response and return the transaction history
                    var parsedData = JsonConvert.DeserializeObject<KrakenTradeHistoryResponse>(response);
                    var tradeHistories = _mapper.Map<List<TradeHistory>>(parsedData.Result.Trades);
                    tradeHistories.ForEach(x => x.UserId = key.UserId);
                    await _localDataRepository.SaveTradeHistories(tradeHistories, CancellationToken.None);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error happened during {action} with {message} : ", nameof(GetTransactionHistoryAsync), ex);
                throw;
            }
           
        }

        public async Task<bool> PostOrder(KrakenOrderRequest orderRequest)
        {
            try
            {
                var key = await _apiKeyRepository.GetApiKeyPerUser(_user.Id, ProviderEnum.Kraken);
                string jsonBody = JsonConvert.SerializeObject(orderRequest);

                // Create StringContent with JSON body
                StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var nonce = GetNonce();
                await _krakenApiClient.PostRequestAsync(_krakenOptions.Value.OrderEndpoint, content, key, nonce);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error happened during {action} with {@request} and {message} : " ,nameof(PostOrder),orderRequest, ex);
                throw;
            }
        }

        public async Task<bool> SyncOrders()
        {
            try
            {
                var ordersToSync = await _localDataRepository.GetOrdersToSync(ProviderEnum.Kraken);
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
                        await _krakenApiClient.PostRequestAsync(_krakenOptions.Value.OrderEndpoint, content, key, nonce);
                    }                   
                }
                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError("An error happened during {action} with {message} : ", nameof(PostOrder), ex);
                throw;
            }
        }

        private long GetNonce()
        {
            return Interlocked.Increment(ref _nonce);
        }
    }
}