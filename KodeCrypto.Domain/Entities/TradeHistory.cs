using KodeCrypto.Domain.Common;
using KodeCrypto.Domain.Entities.Identity;
using KodeCrypto.Domain.Enums;

namespace KodeCrypto.Domain.Entities
{
	public class TradeHistory: BaseAuditableEntity
    {
        public string UserId { get; set; }
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
        public User User { get; set; }
    }
}

