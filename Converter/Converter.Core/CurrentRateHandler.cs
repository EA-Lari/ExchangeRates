using ExchangeTypes;
using ExchangeTypes.Events;
using ExchangeTypes.Request;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Converter.Core
{
    public class CurrentRateHandler : ICurrencyHandler<ConvertCurrencyRequest, ConvertCurrencyResponce> //: ICurrencyHandler<ConvertCurrencyRateEvent>
    {
        private readonly ConverterService _converter;
        private readonly CurrencyPublisher _publisher;

        public CurrentRateHandler(ConverterService converter, CurrencyPublisher publisher)
        {
            _converter = converter;
            _publisher = publisher;
        }

        public async Task<ConvertCurrencyResponce> Handler(ConvertCurrencyRequest @event)
        {
            var allRates = _converter.GetRateAllCurrencies(@event.Currencies);
            return new ConvertCurrencyResponce { CorrelationId = @event.CorrelationId, Currencies = allRates };
        }
    }
}
