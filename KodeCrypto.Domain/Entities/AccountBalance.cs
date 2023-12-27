using KodeCrypto.Domain.Common;
using KodeCrypto.Domain.Entities.Identity;
using KodeCrypto.Domain.Enums;

namespace KodeCrypto.Domain.Entities
{
	public class AccountBalance: BaseAuditableEntity
    {
	    public string UserId { get; set; }
	    public ProviderEnum ProviderId { get; set; }
	    public DateTime SyncDate { get; set; }
	    public string Data { get; set; }

		public User User { get; set; }
	}
}

