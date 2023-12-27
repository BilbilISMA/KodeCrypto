namespace KodeCrypto.Application.DTO.Requests
{
    public class KrakenOrderRequest
    {
        public string Pair { get; set; }
        public string Type { get; set; }
        public string OrderType { get; set; }
        public decimal Price { get; set; }
        public int Volume { get; set; }
        public string Reference { get; set; }
    }
}

