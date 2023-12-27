using KodeCrypto.Application.DTO.Portfolio;
using KodeCrypto.Application.Generic;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KodeCrypto.Application.UseCases.Portfolio.Queries
{
    public class GetAccountBalanceQuery : IRequest<List<AccountBalanceDTO>>
    {
    }
    public class GetAccountBalanceHandler : BaseHandlerRequest<GetAccountBalanceQuery, List<AccountBalanceDTO>>
    {
        public GetAccountBalanceHandler(BaseHandlerServices services) : base(services)
        {
        }

        public override async Task<List<AccountBalanceDTO>> Handle(GetAccountBalanceQuery request, CancellationToken cancellationToken)
        {
            var mappedBalances = new List<AccountBalanceDTO>();

            var balances = await _dbContext.AccountBalances.Where(x => x.UserId == _user.Id).ToListAsync();
            foreach (var balance in balances)
            {
                var mappedBalance = _mapper.Map<AccountBalanceDTO>(balance);

                if (balance.ProviderId == Domain.Enums.ProviderEnum.Kraken)
                {
                    var dictionary = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(balance.Data);

                    List<BalanceDetails> balanceList = new List<BalanceDetails>();
                    foreach (var kvp in dictionary)
                    {
                        double value = Convert.ToDouble(kvp.Value);

                        var balanceDetails = new BalanceDetails
                        {
                            Symbol = kvp.Key,
                            Value = value
                        };
                        balanceList.Add(balanceDetails);
                    }
                    mappedBalance.Details = balanceList;
                    mappedBalances.Add(mappedBalance);
                }
                else
                {
                    mappedBalance.Details = JsonConvert.DeserializeObject<List<BalanceDetails>>(balance.Data);
                    mappedBalances.Add(mappedBalance);
                }
            }
            return mappedBalances;
        }
    }
}
