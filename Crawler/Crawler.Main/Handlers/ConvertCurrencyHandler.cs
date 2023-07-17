using ExchangeTypes.Request;
using ExchangeTypes;
using System.Threading.Tasks;
using Crawler.Core;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Crawler.Main.Handlers
{
    public class GetActualCurrencyHandler : ICurrencyHandler<GetActualCurrencyRequest, GetActualCurrencyResponce>
    {
        private readonly CurrencyService _service;
        private readonly ILogger<GetActualCurrencyHandler> _logger;

        public GetActualCurrencyHandler(CurrencyService service, ILogger<GetActualCurrencyHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<GetActualCurrencyResponce> Handler(GetActualCurrencyRequest @event)
        {

            var result = await _service.GetCurrencyFromCBR();
            return new GetActualCurrencyResponce
            {
                CorrelationId = @event.CorrelationId,
                Currencies = result
            };
        }
    }

    public class ConvertCurrencyHandler : ICurrencyHandler<ConvertCurrencyRequest, ConvertCurrencyResponce>
    {
        private readonly CurrencyService _service;
        private readonly ILogger<ConvertCurrencyHandler> _logger;

        public ConvertCurrencyHandler(CurrencyService service, ILogger<ConvertCurrencyHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<ConvertCurrencyResponce> Handler(ConvertCurrencyRequest @event)
        {

            var result = await _service.GetCurrencyFromCBR();
            return new ConvertCurrencyResponce
            {
                CorrelationId = @event.CorrelationId,
                Currencies = result.Select(x => new Converter.Core.RateCurrency { Name= x.Name}).ToList()
            };
        }
    }
}
