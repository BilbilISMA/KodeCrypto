using Microsoft.EntityFrameworkCore;

namespace KodeCrypto.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Domain.Entities.AccountBalance> AccountBalances { get; }
        DbSet<Domain.Entities.TradeHistory> TradeHistories { get; }
        DbSet<Domain.Entities.ApiKey> ApiKeys { get; }
        DbSet<Domain.Entities.Order> Orders { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}

