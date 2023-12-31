using KodeCrypto.Domain.Common;
using KodeCrypto.Domain.Entities.Identity;

namespace KodeCrypto.Domain.Entities
{
    public class Order : BaseAuditableEntity
    {
        public string UserId { get; set; }
        public string Pair { get; set; }
        public string Type { get; set; }
        public string OrderType { get; set; }
        public decimal Price { get; set; }
        public int Volume { get; set; }
        public string Reference { get; set; }
        public bool Synced { get; set; }

        public User User { get; set; }
    }
}

