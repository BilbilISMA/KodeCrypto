using Newtonsoft.Json;

namespace KodeCrypto.Application.DTO.Responses
{
    public class KrakenBalanceResponse
    {
        public required List<string> Error { get; set; }
        public required KrakenBalanceResult Result { get; set; }
    }

    public class KrakenBalanceResult
    {
        public string ZUSD { get; set; }
        public string ZEUR { get; set; }
        public string XXBT { get; set; }
        public string XETH { get; set; }
        public string USDT { get; set; }
        public string DAI { get; set; }
        public string DOT { get; set; }

        [JsonProperty("ETH2.S")]
        public string ETH2S { get; set; }
        public string ETH2 { get; set; }

        [JsonProperty("USD.M")]
        public string USDM { get; set; }
    }
}

