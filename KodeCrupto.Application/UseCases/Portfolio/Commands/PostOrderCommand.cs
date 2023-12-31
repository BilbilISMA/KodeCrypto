using KodeCrypto.Application.Common.Extensions;
using KodeCrypto.Application.Common.Interfaces;
using KodeCrypto.Application.Generic;
using KodeCrypto.Application.UseCases.APIKey.Commands;
using KodeCrypto.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
        private readonly ILocalDataRepository _localDataRepository;

        public PostOrderHandler(BaseHandlerServices services,
            ILocalDataRepository localDataRepository) : base(services)
        {
            _localDataRepository = localDataRepository;
        }

        public override async Task<bool> Handle(PostOrderCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var order = _mapper.Map<Order>(command);

                var webhooks = typeof(IWebhook).GetImplementations<IWebhook>();

                var tasks = webhooks.Select(webhook => webhook.ProcessOrderAsync(order));
                await Task.WhenAll(tasks);

                bool allSynced = tasks.All(task => task.Result);

                order.Synced = allSynced;
                order.UserId = _user.Id;
                return await _localDataRepository.SaveOrder(order, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error happened during {action} with {@request} and {message} : ", nameof(PostOrderCommand), command, ex);
                throw;
            }            
        }
    }
}