using ExchangeTypes.DTO;
using ExchangeTypes.Request;
using System.Collections.Generic;
using System.Linq;

namespace Converter.Core
{
    public class ConverterService
    {
        /// <summary>
        /// Сompiles the exchange rate for all currencies
        /// </summary>
        /// <param name="rate">Currency by which get rate all currencies</param>
        /// <returns>List currencies with price by all currencies include param "rate"</returns>
        public List<RateCurrency> GetRateAllCurrencies(IList<CurrencyDTO> currencies)
        {
            var result = new List<RateCurrency>(currencies.Count);

            foreach (var currentCurrency in currencies)
            {
                var newCurrency = new RateCurrency
                {
                    Name = currentCurrency.Name
                };
                
                foreach (var otherCurrency in currencies)
                {
                    if (otherCurrency.CharCode == currentCurrency.CharCode) continue;
                    var currencyRate = currentCurrency.Value.Value / otherCurrency.Value.Value;
                    newCurrency.Prices.Add(otherCurrency.Name, currencyRate);
                }
                result.Add(newCurrency);
            }

            return result;
        }
    }
}
