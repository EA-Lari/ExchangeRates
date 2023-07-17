using AutoMapper;
using ExchangeTypes.Events;
using Storage.Database.Models;

namespace Storage.Core
{
    public class CurrancyProfile : Profile
    {
        public CurrancyProfile()
        {
            CreateMap<CurrencyInfoData, Currency>()
                .ForMember(x => x.RId, opt => opt.MapFrom(y => y.ParentCode));
        }
    }
}
