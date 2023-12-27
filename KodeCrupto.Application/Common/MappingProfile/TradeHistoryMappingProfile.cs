using AutoMapper;
using KodeCrypto.Application.DTO.Portfolio;
using KodeCrypto.Application.DTO.Responses;
using KodeCrypto.Domain.Entities;

namespace KodeCrypto.Application.Common.MappingProfile
{
    public class TradeHistoryMappingProfile : Profile
    {
		public TradeHistoryMappingProfile()
        {
            CreateMap<KrakenTradeHistoryResponse.Trade, TradeHistory>()
             .ForMember(dest => dest.UserId, opt => opt.Ignore())  
             .ForMember(dest => dest.ProviderId, opt => opt.Ignore());

            CreateMap<BinanceTransactionHistoryResult, TradeHistory>()
             .ForMember(dest => dest.UserId, opt => opt.Ignore())
             .ForMember(dest => dest.ProviderId, opt => opt.Ignore())
             .ForMember(dest => dest.Type, opt => opt.Ignore())
             .ForMember(dest => dest.Vol, opt => opt.Ignore())
             .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.TimeStamp))
             .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.ListedPrice))
             .ForMember(dest => dest.Misc, opt => opt.MapFrom(src => src.Note));

            CreateMap<TradeHistory, TradeHistoryDTO>();
        }
    }
}

