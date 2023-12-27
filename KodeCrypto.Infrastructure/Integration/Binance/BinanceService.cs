using System.Text;
using AutoMapper;
using KodeCrypto.Application.Common.Interfaces;
using KodeCrypto.Application.DTO.Requests;
using KodeCrypto.Application.DTO.Responses;
using KodeCrypto.Domain.Entities;
using KodeCrypto.Infrastructure.Integration.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace KodeCrypto.Infrastructure.Integration.Binance
{
    public class BinanceService : IBinanceService
    {
        private readonly BinanceApiClient _binanceApiClient;
        private readonly IMapper _mapper;
        private readonly ILocalDataRepository _localDataRepository;
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly IOptions<BinanceConfig> _binanceOptions;
        private readonly ILogger<BinanceService> _logger;

        private readonly IUser _user;
        public BinanceService(BinanceApiClient binanceApiClient, IMapper mapper, ILocalDataRepository localDataRepository, IApiKeyRepository apiKeyRepository, IUser user, IOptions<BinanceConfig> binanceOptions, ILogger<BinanceService> logger)
        {
            _binanceApiClient = binanceApiClient;
            _mapper = mapper;
            _localDataRepository = localDataRepository;
            _apiKeyRepository = apiKeyRepository;
            _user = user;
            _binanceOptions = binanceOptions;
            _logger = logger;
        }

        public async Task<bool> GetBalanceAsync()
        {
            var apiKeys = await _apiKeyRepository.GetApiKeysByProviderId(Domain.Enums.ProviderEnum.Binance);
            foreach (var key in apiKeys)
            {
                var response = await _binanceApiClient.GetRequestAsync(_binanceOptions.Value.BalanceEndpoint, string.Empty, key);

                // Parse the response and return the balance
                var parsedData = JsonConvert.DeserializeObject<BinanceBalanceResponse>(response);
                var accountBalance = _mapper.Map<AccountBalance>(parsedData);
                accountBalance.ProviderId = Domain.Enums.ProviderEnum.Binance;
                accountBalance.UserId = key.UserId;
                await _localDataRepository.SaveAccountBalance(accountBalance, CancellationToken.None);
            }

            return true;
        }

        public async Task<bool> GetTransactionHistoryAsync()
        {
            var apiKeys = await _apiKeyRepository.GetApiKeysByProviderId(Domain.Enums.ProviderEnum.Kraken);
            try
            {
                foreach (var key in apiKeys)
                {
                    var response = await _binanceApiClient.GetRequestAsync(_binanceOptions.Value.TradeBalanceEndpoint, string.Empty, key);
                
                    // Parse the response and return the transaction history
                    var parsedData = JsonConvert.DeserializeObject<BinanceTradeHistoryResponse>(response);
                    var tradeHistories = _mapper.Map<List<TradeHistory>>(parsedData.Result);
                    tradeHistories.ForEach(x => { x.UserId = key.UserId; x.ProviderId = Domain.Enums.ProviderEnum.Binance; });

                    _logger.LogInformation("Saving trade with {@Content} using {Key}", tradeHistories, key);

                    await _localDataRepository.SaveTradeHistories(tradeHistories, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error happened during {nameof(GetTransactionHistoryAsync)} with message : ", ex);
                throw;
            }
            return true;
        }


        public async Task<bool> PostOrder(BinanceOrderRequest orderRequest)
        {
            try
            {
                var key = await _apiKeyRepository.GetApiKeyPerUser(_user.Id, Domain.Enums.ProviderEnum.Binance);
                string jsonBody = JsonConvert.SerializeObject(orderRequest);

                // Create StringContent with JSON body
                StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                _logger.LogInformation("Posting Order with {@Content} using {Key}", content, key);

                return await _binanceApiClient.PostRequestAsync(_binanceOptions.Value.OrderEndpoint, content, key);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error happened during {nameof(PostOrder)} with message : " ,ex);
                throw;
            }           
        }
    }
}