using ExchangeTypes.Request;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ExchangeTypes.Consumers
{
    public class ConvertCurrencyConsumer : IConsumer<ConvertCurrencyRequest>
    {
        readonly ILogger<ConvertCurrencyConsumer> _logger;
        readonly ICurrencyHandler<ConvertCurrencyRequest, ConvertCurrencyResponce> _convertCurrencyService;

        public ConvertCurrencyConsumer(ILogger<ConvertCurrencyConsumer> logger, ICurrencyHandler<ConvertCurrencyRequest, ConvertCurrencyResponce> actualCurrencyService)
        {
            _logger = logger;
            _convertCurrencyService = actualCurrencyService;
        }

        public Task Consume(ConsumeContext<ConvertCurrencyRequest> context)
        {
            _logger.LogInformation($"Get Request:{typeof(ConvertCurrencyRequest)}");
            var taskResult = _convertCurrencyService.Handler(context.Message);
            taskResult.Wait();
            var result = taskResult.Result;
            return context.RespondAsync(new ConvertCurrencyResponce
            {
                CorrelationId = context.Message.CorrelationId,
                Currencies = result.Currencies
            });
        }
    }
}
