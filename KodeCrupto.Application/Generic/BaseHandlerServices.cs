using AutoMapper;
using KodeCrypto.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace KodeCrypto.Application.Generic
{
    public class BaseHandlerServices
	{
        public readonly IMapper Mapper;
        public readonly ILoggerFactory LogerFactory;
        public readonly IApplicationDbContext DbContext;
        public readonly IIdentityService IdentityService;
        public readonly IUser User;

        public BaseHandlerServices(IMapper mapper,
                                   ILoggerFactory loggerFactory,
                                   IApplicationDbContext dbContext,
                                   IIdentityService identityService,
                                   IUser user)
        {
            Mapper = mapper;
            DbContext = dbContext;
            LogerFactory = loggerFactory;
            IdentityService = identityService;
            User = user;
        }
    }
}

