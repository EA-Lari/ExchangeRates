using AutoMapper;
using Crawler.Core.DTO;
using ExchangeTypes.DTO;
using ExchangeTypes.Events;

namespace Crawler.Core
{
    public class CurrancyProfile : Profile
    {
        public CurrancyProfile()
        {
            CreateMap<CurrencyItemInfoDTO, ActualCurrencyFromWebDto>()
                .ForMember(x => x.IsoCode, opt => opt.MapFrom(z => z.IsoCharCode));
            CreateMap<CurrencyItemInfoDTO, CurrencyInfoData>()
                .ForMember(x => x.IsoCode, opt => opt.MapFrom(z => z.IsoCharCode));
            CreateMap<CurrencyValueDTO, PriceInCurrencyData>()
                .ForMember(x => x.IsoCharCode, opt => opt.MapFrom(z => z.CharCode));
        }
    }
}
