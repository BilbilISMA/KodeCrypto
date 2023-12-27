using KodeCrypto.Application.Common.Interfaces;
using KodeCrypto.Application.DTO.Requests;
using KodeCrypto.Application.Generic;
using KodeCrypto.Domain.Entities;
using MediatR;

namespace KodeCrypto.Application.UseCases.Portfolio.Commands
{
    public class PostOrderCommand : IRequest<bool>
    {
        public required string Pair { get; set; }
        public required string Type { get; set; }
        public required string OrderType { get; set; }
        public decimal Price { get; set; }
        public int Volume { get; set; }
        public required string Reference { get; set; }
    }

    public class PostOrderHandler : BaseHandlerRequest<PostOrderCommand, bool>
    {
        private readonly IBinanceService _binanceService;
        private readonly IKrakenService _krakenService;
        private readonly ILocalDataRepository _localDataRepository;

        public PostOrderHandler(BaseHandlerServices services, IBinanceService binanceService, IKrakenService krakenService, ILocalDataRepository localDataRepository) : base(services)
        {
            _binanceService = binanceService;
            _krakenService = krakenService;
            _localDataRepository = localDataRepository;
        }

        public override async Task<bool> Handle(PostOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var order = _mapper.Map<Order>(request);

                var krakenOrder = _mapper.Map<KrakenOrderRequest>(order);
                var savedInKraken = await _krakenService.PostOrder(krakenOrder);

                var binanceOrder = _mapper.Map<BinanceOrderRequest>(order);
                var savedInBinance = await _binanceService.PostOrder(binanceOrder);

                order.SyncedToBinance = savedInBinance;
                order.SyncedToKraken = savedInKraken;
                order.UserId = _user.Id;
                await _localDataRepository.SaveOrder(order, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }            
        }
    }
}