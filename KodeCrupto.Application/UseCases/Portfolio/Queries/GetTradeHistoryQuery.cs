using KodeCrypto.Application.DTO.Portfolio;
using KodeCrypto.Application.Generic;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KodeCrypto.Application.UseCases.Portfolio.Queries
{
    public class GetTradeHistoryQuery : IRequest<List<TradeHistoryDTO>>
    {
    }
    public class GetTradeHistoryHandler : BaseHandlerRequest<GetTradeHistoryQuery, List<TradeHistoryDTO>>
    {
        public GetTradeHistoryHandler(BaseHandlerServices services) : base(services)
        {
        }

        public override async Task<List<TradeHistoryDTO>> Handle(GetTradeHistoryQuery request, CancellationToken cancellationToken)
        {
            var balances = await _dbContext.TradeHistories.Where(x => x.UserId == _user.Id).ToListAsync();

            return _mapper.Map<List<TradeHistoryDTO>>(balances);
        }
    }
}
