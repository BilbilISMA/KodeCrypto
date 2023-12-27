using AutoMapper;
using KodeCrypto.Application.DTO.Portfolio;
using KodeCrypto.Application.DTO.Responses;
using KodeCrypto.Domain.Entities;
using Newtonsoft.Json;

namespace KodeCrypto.Application.Common.MappingProfile
{
    public class AccountBalanceMappingProfile : Profile
    {
		public AccountBalanceMappingProfile()
        {
            CreateMap<KrakenBalanceResponse, AccountBalance>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.ProviderId, opt => opt.Ignore())
                .ForMember(dest => dest.SyncDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Data, opt => opt.MapFrom<KrakenJsonResultResolver>());

            CreateMap<BinanceBalanceResponse, AccountBalance>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.ProviderId, opt => opt.Ignore())
                .ForMember(dest => dest.SyncDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Data, opt => opt.MapFrom<BinanceJsonResultResolver>());

            CreateMap<AccountBalance, AccountBalanceDTO>()
                .ForMember(dest => dest.Details, opt => opt.Ignore());
        }

        private class KrakenJsonResultResolver : IValueResolver<KrakenBalanceResponse, AccountBalance, string>
        {
            public string Resolve(KrakenBalanceResponse source, AccountBalance destination, string destMember, ResolutionContext context)
            {
                return JsonConvert.SerializeObject(source.Result);
            }
        }

        private class BinanceJsonResultResolver : IValueResolver<BinanceBalanceResponse, AccountBalance, string>
        {
            public string Resolve(BinanceBalanceResponse source, AccountBalance destination, string destMember, ResolutionContext context)
            {
                return JsonConvert.SerializeObject(source.Data);
            }
        }
    }
}

