using ExchangeTypes;
using ExchangeTypes.Events;
using ExchangeTypes.Request;
using Microsoft.Extensions.Logging;
using Storage.Core.Repositories;
using System.Threading.Tasks;

namespace Storage.Core.Handlers
{
    public class UpdateCurrencyInfoHandler : ICurrencyHandler<UpdateCurrencyRequest, UpdateCurrencyResponce>
    {
        private readonly ILogger<UpdateCurrencyInfoHandler> _logger;
        private readonly CurrencyRatesRepository _repository;

        public UpdateCurrencyInfoHandler(ILogger<UpdateCurrencyInfoHandler> logger, CurrencyRatesRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task<UpdateCurrencyResponce> Handler(UpdateCurrencyRequest @event)
        {
            _logger.LogInformation("Start update Currency");
            var result = await _repository.SaveCurrencyInfo(@event.Currencies);
            return new UpdateCurrencyResponce
            {
                CorrelationId = @event.CorrelationId,
                Currencies = result
            };
        }
    }
}
