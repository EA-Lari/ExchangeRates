using ExchangeTypes.DTO;
using ExchangeTypes;
using System.Threading.Tasks;
using Storage.Core.Repositories;

namespace Storage.Core.Handlers
{
    public class GetCurrencyHandler : ICurrencyHandler<FilterByCurrencyDto, CurrencyRateResponce>
    {
        private readonly CurrencyRatesRepository _repository;

        public GetCurrencyHandler(CurrencyRatesRepository repository)
        {
            _repository = repository;
        }

        public async Task<CurrencyRateResponce> Handler(FilterByCurrencyDto @event)
        {
            return new CurrencyRateResponce { Currencies=await _repository.GetCurrencies(@event) };
        }
    }
}