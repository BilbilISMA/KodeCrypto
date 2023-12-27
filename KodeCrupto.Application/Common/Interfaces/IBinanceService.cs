using KodeCrypto.Application.DTO.Requests;

namespace KodeCrypto.Application.Common.Interfaces
{
    public interface IBinanceService
    {
        Task<bool> GetBalanceAsync();
        Task<bool> GetTransactionHistoryAsync();
        Task<bool> PostOrder(BinanceOrderRequest orderRequest);
    }
}

