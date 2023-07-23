using Crawler.Core;
using ExchangeTypes;
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
        private readonly CurrencyService _service;
        //private readonly CurrencyPublisher _publisher;
        private readonly IBus _bus;
        private readonly ILogger<CurrencyController> _logger;

        public CurrencyController(CurrencyService service, ILogger<CurrencyController> logger, IBus bus)
        {
            _service = service;
            // _publisher = publisher;
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
           // _bus.GetSendEndpoint(new Uri("saga"));
            await _bus.Publish(update);
            // await _publisher.Publish(update);
            _logger.LogInformation($"End {nameof(UpdateCurrencyInfoEvent)}");
            return Ok(update.CorrelationId);
        }
    }
}
