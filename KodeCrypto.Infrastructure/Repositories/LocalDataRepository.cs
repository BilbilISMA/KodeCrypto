using KodeCrypto.Application.Common.Interfaces;
using KodeCrypto.Domain.Entities;
using KodeCrypto.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace KodeCrypto.Infrastructure.Repositories
{
    public class LocalDataRepository : ILocalDataRepository
    {
		private readonly IApplicationDbContext _dbContext;
        private readonly IUser _user;
        private readonly ILogger<LocalDataRepository> _logger;

        public LocalDataRepository(IApplicationDbContext dbContext, IUser user, ILogger<LocalDataRepository> logger)
        {
            _dbContext = dbContext;
            _user = user;
            _logger = logger;
        }

        public async Task<List<AccountBalance>> GetUserAccountBalance()
        {
            try
            {
                return _dbContext.AccountBalances.Where(x => x.UserId == _user.Id).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error happened during {@action} with {@userId} and {@Message} : ", nameof(GetUserAccountBalance), _user.Id, ex);

                throw;
            }
        }

        public async Task<List<TradeHistory>> GetUserTradeHistory()
        {
            try
            {
                return _dbContext.TradeHistories.Where(x => x.UserId == _user.Id).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error happened during {@action} with {@userId} and {@Message} : ", nameof(GetUserTradeHistory), _user.Id, ex);

                throw;
            }
        }

        public async Task<bool> SaveAccountBalance(AccountBalance accountBalance, CancellationToken cancellationToken)
		{
			try
			{
                var existingBalance = _dbContext.AccountBalances.FirstOrDefault(x => x.UserId == accountBalance.UserId && x.ProviderId == accountBalance.ProviderId);
                if (existingBalance is null)
                {
                    await _dbContext.AccountBalances.AddAsync(accountBalance);
                }
                else
                {
                    existingBalance.Data = accountBalance.Data;
                    _dbContext.AccountBalances.Update(existingBalance);
                }
                await _dbContext.SaveChangesAsync(cancellationToken);

                return true;
            }
			catch (Exception ex)
			{
                _logger.LogError("An error happened during {@action} with {@request} and {@Message} : ", nameof(SaveAccountBalance), accountBalance, ex);

                throw;
			}			
		}
        
        public async Task<bool> SaveTradeHistories(List<TradeHistory> tradeHistories, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var tradeHistory in tradeHistories)
                {
                    var existingHistory = _dbContext.TradeHistories.FirstOrDefault(x => x.UserId == tradeHistory.UserId && x.ProviderId == tradeHistory.ProviderId && x.TradeId == tradeHistory.TradeId);
                    if (existingHistory is null)
                    {
                        await _dbContext.TradeHistories.AddAsync(tradeHistory);
                    }
                    else
                    {
                        existingHistory.Pair = tradeHistory.Pair;
                        existingHistory.Time = tradeHistory.Time;
                        existingHistory.Price = tradeHistory.Price;
                        existingHistory.Cost = tradeHistory.Cost;
                        existingHistory.Fee = tradeHistory.Fee;
                        existingHistory.Margin = tradeHistory.Margin;
                        existingHistory.Misc = tradeHistory.Misc;

                        _dbContext.TradeHistories.Update(existingHistory);
                    }
                }
                await _dbContext.SaveChangesAsync(cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error happened during {@action} with {@request} and {@Message} : ", nameof(SaveTradeHistories), tradeHistories, ex);

                throw;
            }
        }

        public async Task<bool> SaveOrder(Order order, CancellationToken cancellationToken )
        {
            try
            {
                await _dbContext.Orders.AddAsync(order);
                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.LogInformation($"{nameof(SaveOrder)} with @Content", SaveOrder);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error happened during {@action} with {@request} and {@Message} : ", nameof(SaveOrder) ,order, ex);

                throw;
            }
        }

        public async Task<List<Order>> GetOrdersToSync(ProviderEnum providerId)
        {
            try
            {
                return _dbContext.Orders.Where(x => providerId == ProviderEnum.Binance ? !x.SyncedToBinance : !x.SyncedToKraken).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error happened during {@action} with {@Message} : ", nameof(GetOrdersToSync), ex);

                throw;
            }
        }
    }
}

