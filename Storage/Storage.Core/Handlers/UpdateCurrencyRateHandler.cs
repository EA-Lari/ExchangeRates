using ExchangeTypes;
using ExchangeTypes.Request;
using Microsoft.Extensions.Logging;
using Storage.Core.Repositories;
using System.Threading.Tasks;

namespace Storage.Core.Handlers
{
    public class UpdateCurrencyRateHandler : ICurrencyHandler<UpdateRatesRequest, UpdateRatesResponce>
    {
        private readonly ILogger<UpdateCurrencyRateHandler> _logger;
        private readonly CurrencyRatesRepository _repository;

        public UpdateCurrencyRateHandler(CurrencyRatesRepository repository, ILogger<UpdateCurrencyRateHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<UpdateRatesResponce> Handler(UpdateRatesRequest @event)
        {
            _logger.LogInformation("Start save Rate");
            await _repository.SaveCurrencyRates(@event.Currencies);
            return new UpdateRatesResponce
            {
                CorrelationId = @event.CorrelationId
            };
        }
    }
}
