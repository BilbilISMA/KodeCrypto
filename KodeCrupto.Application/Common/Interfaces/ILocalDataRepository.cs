using KodeCrypto.Domain.Entities;
using KodeCrypto.Domain.Enums;

namespace KodeCrypto.Application.Common.Interfaces
{
    public interface ILocalDataRepository
    {
        Task<List<AccountBalance>> GetUserAccountBalance();
        Task<List<TradeHistory>> GetUserTradeHistory();

        Task<bool> SaveAccountBalance(AccountBalance accountBalance, CancellationToken cancellationToken);
        Task<bool> SaveTradeHistories(List<TradeHistory> tradeHistories, CancellationToken cancellationToken);
        Task<bool> SaveOrder(Order order, CancellationToken cancellationToken);
        Task<List<Order>> GetOrdersToSync(ProviderEnum providerId);
    }
}

