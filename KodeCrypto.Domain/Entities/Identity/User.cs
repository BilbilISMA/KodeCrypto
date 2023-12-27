using Microsoft.AspNetCore.Identity;

namespace KodeCrypto.Domain.Entities.Identity
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public bool IsStaff { get; set; }
        public bool IsCompany { get; set; }
        public bool IsSuperUser { get; set; }
        public string? RefreshToken { get; set; }
        public string Token { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string? PasswordRecoveyToken { get; set; }
        public DateTime? PasswordRecoveyTokenCreationTime { get; set; }
        public IEnumerable<ApiKey> ApiKeys { get; set; }
        public IEnumerable<AccountBalance> AccountBalances { get; set; }
        public IEnumerable<TradeHistory> TradeHistories { get; set; }
        public IEnumerable<Order> Orders { get; set; }

    }
}
