using ExchangeTypes.Request;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ExchangeTypes.Consumers
{
    public class UpdateCurrencyConsumer : IConsumer<UpdateCurrencyRequest>
    {
        readonly ILogger<UpdateCurrencyConsumer> _logger;
        readonly ICurrencyHandler<UpdateCurrencyRequest, UpdateCurrencyResponce> _convertCurrencyService;

        public UpdateCurrencyConsumer(ILogger<UpdateCurrencyConsumer> logger, ICurrencyHandler<UpdateCurrencyRequest, UpdateCurrencyResponce> actualCurrencyService)
        {
            _logger = logger;
            _convertCurrencyService = actualCurrencyService;
        }

        public async Task Consume(ConsumeContext<UpdateCurrencyRequest> context)
        {
            _logger.LogInformation($"Get Request:{typeof(UpdateCurrencyRequest)}");
            var result = await _convertCurrencyService.Handler(context.Message);
            await context.RespondAsync(result);
        }
    }
}
