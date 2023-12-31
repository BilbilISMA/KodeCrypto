namespace KodeCrypto.Application.Common.Interfaces
{
    public interface ISyncService
    {
        Task<bool> SyncAll();
        Task<bool> SyncBalance();
        Task<bool> SyncTransactionHistory();
        Task<bool> SyncOrders();
    }
}

