using KodeCrypto.Application.DTO.Requests;

namespace KodeCrypto.Application.Common.Interfaces
{
    public interface IKrakenService
    {
        Task<bool> GetBalanceAsync();
        Task<bool> GetTransactionHistoryAsync();
        Task<bool> PostOrder(KrakenOrderRequest orderRequest);
        Task<bool> SyncOrders();
    }
}
