using System.Security.Cryptography;
using System.Text;
using KodeCrypto.Domain.Entities;
using KodeCrypto.Infrastructure.Integration.Configurations;
using Microsoft.Extensions.Options;

namespace KodeCrypto.Infrastructure.Integration.Binance
{
    public class BinanceApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<BinanceConfig> _binanceOptions;

        public BinanceApiClient(HttpClient httpClient, IOptions<BinanceConfig> binanceOptions)
        {
            _httpClient = httpClient;
            _binanceOptions = binanceOptions;
            _httpClient.BaseAddress = new Uri(_binanceOptions.Value.BaseAddress);
        }

        public async Task<string> GetRequestAsync(string endpoint, string queryString, ApiKey apiKey)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            var signature = GenerateSignature(queryString, timestamp, apiKey.Secret);

            var requestUri = $"{endpoint}?{queryString}";
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey.Key);
            _httpClient.DefaultRequestHeaders.Add("X-MBX-SIGN", signature);

            var response = await _httpClient.GetAsync(requestUri);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<bool> PostRequestAsync(string endpoint, StringContent body, ApiKey apiKey)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            var signature = GenerateSignature(string.Empty, timestamp, apiKey.Secret);

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", apiKey.Key);
            _httpClient.DefaultRequestHeaders.Add("X-MBX-SIGN", signature);

            var response = await _httpClient.PostAsync(endpoint, body);

            return response.IsSuccessStatusCode;
        }

        private string GenerateSignature(string queryString, string timestamp, string secret)
        {
            var data = $"{queryString}&timestamp={timestamp}";
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using var hmac = new HMACSHA256(keyBytes);
            var hash = hmac.ComputeHash(dataBytes);

            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}