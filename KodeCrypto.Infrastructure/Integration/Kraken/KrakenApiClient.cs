using System.Security.Cryptography;
using System.Text;
using KodeCrypto.Domain.Entities;
using KodeCrypto.Infrastructure.Integration.Configurations;
using Microsoft.Extensions.Options;

namespace KodeCrypto.Infrastructure.Integration.Kraken
{
    public class KrakenApiClient
	{
        private readonly HttpClient _httpClient;
        private readonly IOptions<KrakenConfig> _krakenOptions;

        public KrakenApiClient(HttpClient httpClient, IOptions<KrakenConfig> krakenOptions)
        {
            _httpClient = httpClient;
            _krakenOptions = krakenOptions;
        }

        public async Task<string> PostRequestAsync(string endpoint, StringContent body, ApiKey apiKey,long? nonce = null)
        {
            var postData = $"nonce={nonce}";

            var requestUri = $"{endpoint}";

            _httpClient.BaseAddress = new Uri(_krakenOptions.Value.BaseAddress);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("API-Key", apiKey.Key);
            _httpClient.DefaultRequestHeaders.Add("API-Sign", GenerateSignature(endpoint, nonce.Value, postData, apiKey.Secret));

            var response = await _httpClient.PostAsync(requestUri, body);

            // Handle the response and return data
            return await response.Content.ReadAsStringAsync();
        }

        private string GenerateSignature(string endpoint, long nonce, string postData, string secret)
        {
            var path = $"/0{endpoint}";
            var message = $"{path}{nonce}{postData}";

            var secretBytes = Convert.FromBase64String(secret);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            using var hmacsha256 = new HMACSHA256(secretBytes);
            var hashBytes = hmacsha256.ComputeHash(messageBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}
