using ExchangeTypes.DTO;
using ExchangeTypes.Request;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ExchangeTypes.Consumers
{
    public class GetCurrencyConsumer : IConsumer<FilterByCurrencyDto>
    {
        readonly ILogger<GetCurrencyConsumer> _logger;
        readonly ICurrencyHandler<FilterByCurrencyDto, CurrencyRateResponce> _convertCurrencyService;

        public GetCurrencyConsumer(ILogger<GetCurrencyConsumer> logger, ICurrencyHandler<FilterByCurrencyDto, CurrencyRateResponce> actualCurrencyService)
        {
            _logger = logger;
            _convertCurrencyService = actualCurrencyService;
        }

        public async Task Consume(ConsumeContext<FilterByCurrencyDto> context)
        {
            _logger.LogInformation($"Get Request:{typeof(FilterByCurrencyDto)}");
            var result = await _convertCurrencyService.Handler(context.Message);
            await context.RespondAsync(result);
            _logger.LogInformation($"Send Request:{typeof(CurrencyRateResponce)}");
        }
    }
}
