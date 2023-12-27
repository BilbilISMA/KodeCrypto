using System;
namespace KodeCrypto.Infrastructure.Integration.Configurations
{
	public class KrakenConfig
	{
        public const string SettingsSection = "Kraken";

        public string BalanceEndpoint { get; set; }
        public string TradeBalanceEndpoint { get; set; }
        public string BaseAddress { get; set; }
    }
}

