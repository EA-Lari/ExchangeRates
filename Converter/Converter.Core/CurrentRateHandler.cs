﻿using ExchangeTypes;
using ExchangeTypes.Request;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Converter.Core
{
    public class CurrentRateHandler : ICurrencyHandler<ConvertCurrencyRequest, ConvertCurrencyResponce> 
    {
        private readonly IConverterService _converter;
        private readonly ILogger<CurrentRateHandler> _logger;

        public CurrentRateHandler(IConverterService converter, ILogger<CurrentRateHandler> logger)
        {
            _converter = converter;
            _logger = logger;
        }

        public async Task<ConvertCurrencyResponce> Handler(ConvertCurrencyRequest @event)
        {
            _logger.LogInformation("Start convert Rates");
            var allRates = _converter.GetRateAllCurrencies(@event.Currencies);
            return new ConvertCurrencyResponce { CorrelationId = @event.CorrelationId, Currencies = allRates };
        }
    }
}
