﻿using KodeCrypto.Domain.Enums;

namespace KodeCrypto.Application.DTO.Portfolio
{
    public class TradeHistoryDTO
	{
        public string TradeId { get; set; }
        public string Pair { get; set; }
        public double Time { get; set; }
        public string? Type { get; set; }
        public string OrderType { get; set; }
        public string Price { get; set; }
        public string Cost { get; set; }
        public string Fee { get; set; }
        public string? Vol { get; set; }
        public string Margin { get; set; }
        public string Misc { get; set; }
        public ProviderEnum ProviderId { get; set; }
    }
}
