using System;
using AutoMapper;
using KodeCrypto.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KodeCrypto.Application.Generic
{
    public abstract class BaseHandlerRequest<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        protected readonly IMapper _mapper;
        protected readonly ILogger _logger;
        protected readonly IApplicationDbContext _dbContext;
        protected readonly IUser _user;

        protected BaseHandlerRequest(BaseHandlerServices services)
        {
            _mapper = services.Mapper;
            _logger = services.LogerFactory.CreateLogger(GetType());
            _dbContext = services.DbContext;
            _user = services.User;
        }

        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}

