using KodeCrypto.Domain.Common;
using KodeCrypto.Domain.Entities.Identity;
using KodeCrypto.Domain.Enums;

namespace KodeCrypto.Domain.Entities
{
	public class ApiKey: BaseEntity
    {
        public string UserId { get; set; }
        public ProviderEnum ProviderId { get; set; }

        public string Key { get; set; }
        public string Secret { get; set; }

        public User User { get; set; }
    }
}

