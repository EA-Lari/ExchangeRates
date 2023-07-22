using ExchangeTypes.Request;
using ExchangeTypes;
using System.Threading.Tasks;
using Crawler.Core;
using Microsoft.Extensions.Logging;

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
}
