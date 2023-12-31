using KodeCrypto.Domain.Entities;

namespace KodeCrypto.Application.Common.Interfaces
{
    public interface IWebhook
	{
        Task<bool> ProcessOrderAsync(Order order);
    }
}

