using ExchangeTypes.Request;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ExchangeTypes.Consumers
{
    public class ActualCurrencyConsumer : IConsumer<GetActualCurrencyRequest>
    {
        readonly ILogger<ActualCurrencyConsumer> _logger;
        readonly ICurrencyHandler<GetActualCurrencyRequest, GetActualCurrencyResponce> _actualCurrencyService;

        public ActualCurrencyConsumer(ILogger<ActualCurrencyConsumer> logger, ICurrencyHandler<GetActualCurrencyRequest, GetActualCurrencyResponce> actualCurrencyService)
        {
            _logger = logger;
            _actualCurrencyService = actualCurrencyService;
        }

        public async Task Consume(ConsumeContext<GetActualCurrencyRequest> context)
        {
            _logger.LogInformation($"Get Request:{typeof(GetActualCurrencyRequest)}");
            var result = await _actualCurrencyService.Handler(context.Message);
            await context.RespondAsync(result);
        }
    }
}
