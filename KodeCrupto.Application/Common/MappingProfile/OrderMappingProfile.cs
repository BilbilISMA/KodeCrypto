using AutoMapper;
using KodeCrypto.Application.DTO.Requests;
using KodeCrypto.Application.UseCases.Portfolio.Commands;
using KodeCrypto.Domain.Entities;

namespace KodeCrypto.Application.Common.MappingProfile
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<KrakenOrderRequest, Order>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.SyncedToBinance, opt => opt.Ignore())
                .ForMember(dest => dest.SyncedToKraken, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<BinanceOrderRequest, Order>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.SyncedToBinance, opt => opt.Ignore())
                .ForMember(dest => dest.SyncedToKraken, opt => opt.Ignore())
                .ForMember(dest => dest.OrderType, opt => opt.Ignore())
                .ForMember(dest => dest.Reference, opt => opt.MapFrom(src => src.UniRef))
                .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => src.Amount))
                .ReverseMap();

            CreateMap<PostOrderCommand, Order>().ReverseMap();
        }
    }
}

