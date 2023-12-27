using Newtonsoft.Json;

namespace KodeCrypto.Application.DTO.Responses
{
    public class BinanceBalanceResponse
    {
        public required List<string> Error { get; set; }
        public required List<BinanceBalanceData> Data { get; set; }
    }

    public class BinanceBalanceData
    {
        public string Symbol { get; set; }
        public string Value { get; set; }
    }
}

