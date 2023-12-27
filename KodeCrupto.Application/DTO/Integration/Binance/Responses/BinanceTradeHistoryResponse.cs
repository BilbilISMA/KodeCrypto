namespace KodeCrypto.Application.DTO.Responses
{
    public class BinanceTradeHistoryResponse
    {
        public List<string> Error { get; set; }
        public List<BinanceTransactionHistoryResult> Result { get; set; }
    }

    public class BinanceTransactionHistoryResult
    {
        public string TradeId { get; set; }
        public string OrderId { get; set; }
        public string Pair { get; set; }
        public double TimeStamp { get; set; }
        public string Ordertype { get; set; }
        public string ListedPrice { get; set; }
        public string Cost { get; set; }
        public string Fee { get; set; }
        public string Margin { get; set; }
        public string Note { get; set; }
    }
}

