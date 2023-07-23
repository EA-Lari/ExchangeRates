using ExchangeTypes.Request;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ExchangeTypes.Consumers
{
    public class UpdateRatesConsumer : IConsumer<UpdateRatesRequest>
    {
        readonly ILogger<UpdateRatesConsumer> _logger;
        readonly ICurrencyHandler<UpdateRatesRequest, UpdateRatesResponce> _convertCurrencyService;

        public UpdateRatesConsumer(ILogger<UpdateRatesConsumer> logger, ICurrencyHandler<UpdateRatesRequest, UpdateRatesResponce> actualCurrencyService)
        {
            _logger = logger;
            _convertCurrencyService = actualCurrencyService;
        }

        public async Task Consume(ConsumeContext<UpdateRatesRequest> context)
        {
            _logger.LogInformation($"Get Request:{typeof(UpdateRatesRequest)}");
            var result = await _convertCurrencyService.Handler(context.Message);
            await context.Publish(result);
        }
    }
}
