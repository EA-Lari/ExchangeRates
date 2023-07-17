using Microsoft.AspNetCore.Mvc;
using Storage.Core.Services;
using Storage.Core.DTO;

namespace Storage.Main.Controllers
{
    public class UpdateCurrenciesController : Controller
    {
        private readonly CurrencyService _service;

        public UpdateCurrenciesController(CurrencyService service)
        {
            _service = service;
        }

        public IActionResult GetCurrencies(FilterByCurrencyDTO filter)
        {
            var currencies = _service.GetCurrencies(filter);
            return Ok(currencies);
        }
    }
}
