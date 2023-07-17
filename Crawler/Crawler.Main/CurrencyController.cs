using Crawler.Core;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Crawler.Main
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly CurrencyService _service;

        public CurrencyController(CurrencyService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrencyDirectRequest()
        {
            var currencies = await _service.GetCurrencyFromCBR();
            return Ok(currencies);
        }
    }
}
