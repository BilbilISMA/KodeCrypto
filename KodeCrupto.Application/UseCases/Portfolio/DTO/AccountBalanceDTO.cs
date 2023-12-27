using KodeCrypto.Domain.Enums;

namespace KodeCrypto.Application.DTO.Portfolio
{
    public class AccountBalanceDTO
	{
        public ProviderEnum ProviderId { get; set; }
        public DateTime SyncDate { get; set; }
        public List<BalanceDetails> Details { get; set; }
    }

    public class BalanceDetails
    {
        public string Symbol { get; set; }
        public double Value { get; set; }
    }
}

