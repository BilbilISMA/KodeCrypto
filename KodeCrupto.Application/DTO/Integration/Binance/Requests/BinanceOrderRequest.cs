namespace KodeCrypto.Application.DTO.Requests
{
    public class BinanceOrderRequest
    {
        public string Pair { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public int Amount { get; set; }
        public string UniRef { get; set; }
    }
}

