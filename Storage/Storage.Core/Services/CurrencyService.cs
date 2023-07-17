using Storage.Core.DTO;
using Storage.Core.Repositories;
using Storage.Database.Models;
using System.Collections.Generic;

namespace Storage.Core.Services
{
    public class CurrencyService
    {
        private readonly CurrencyRatesRepository _repository;

        public List<CurrencyRate> GetCurrencies(FilterByCurrencyDTO filter)
            => _repository.GetCurrencies(filter);
    }
}
