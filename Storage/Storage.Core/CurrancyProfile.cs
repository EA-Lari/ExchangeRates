using AutoMapper;
using ExchangeTypes.DTO;
using ExchangeTypes.Events;
using Storage.Database.Models;

namespace Storage.Core
{
    public class CurrancyProfile : Profile
    {
        public CurrancyProfile()
        {
            CreateMap<ActualCurrencyFromWebDto, Currency>();
                //.ForMember(x => x.RId, opt => opt.MapFrom(y => y.ParentCode));
        }
    }
}
