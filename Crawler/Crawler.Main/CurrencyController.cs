using Crawler.Core;
using ExchangeTypes;
using ExchangeTypes.DTO;
using ExchangeTypes.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace Crawler.Main
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _service;
        private readonly IBus _bus;
        private readonly ILogger<CurrencyController> _logger;

        public CurrencyController(ICurrencyService service, ILogger<CurrencyController> logger, IBus bus)
        {
            _service = service;
            _logger = logger;
            _bus = bus;
            _bus.GetSendEndpoint(new Uri("queue:service"));
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrencyDirectRequest()
        {
            var currencies = await _service.GetCurrencyFromCBR();
            return Ok(currencies);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCurrencies()
        {
            _logger.LogInformation($"Publish {nameof(UpdateCurrencyInfoEvent)}");
            var update = new UpdateCurrencyInfoEvent { CorrelationId = System.Guid.NewGuid() };
            await _bus.Publish(update);
            _logger.LogInformation($"End {nameof(UpdateCurrencyInfoEvent)}");
            return Ok(update.CorrelationId);
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrencies()
        {
            _logger.LogInformation($"Publish {nameof(FilterByCurrencyDto)}");
            var update = new UpdateCurrencyInfoEvent { CorrelationId = System.Guid.NewGuid() };
            var currency = await _bus.Request<FilterByCurrencyDto, CurrencyRateResponce>(new FilterByCurrencyDto());
            _logger.LogInformation($"End {nameof(FilterByCurrencyDto)}");
            return Ok(currency);
        }
    }
}
