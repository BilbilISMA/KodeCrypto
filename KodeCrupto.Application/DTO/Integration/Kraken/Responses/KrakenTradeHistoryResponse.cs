namespace KodeCrypto.Application.DTO.Responses
{
    public class KrakenTradeHistoryResponse
    {
        public List<object> Error { get; set; }
        public TradeHistory Result { get; set; }

        public class TradeHistory
        {
            public List<Trade> Trades { get; set; }
        }

        public class Trade
        {
            public string TradeId { get; set; }
            public string OrdertxId { get; set; }
            public string PostxId { get; set; }
            public string Pair { get; set; }
            public double Time { get; set; }
            public string Type { get; set; }
            public string OrderType { get; set; }
            public string Price { get; set; }
            public string Cost { get; set; }
            public string Fee { get; set; }
            public string Vol { get; set; }
            public string Margin { get; set; }
            public string Misc { get; set; }
        }
    }
}

