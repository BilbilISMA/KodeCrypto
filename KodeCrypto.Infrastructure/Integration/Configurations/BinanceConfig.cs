namespace KodeCrypto.Infrastructure.Integration.Configurations
{
    public class BinanceConfig
    {
        public const string SettingsSection = "Binance";

        public string BalanceEndpoint { get; set; }
        public string TradeBalanceEndpoint { get; set; }
        public string BaseAddress { get; set; }
        public string OrderEndpoint { get; set; }
    }
}
