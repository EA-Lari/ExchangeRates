using ExchangeTypes.DTO;
using System.Collections.Generic;

namespace Converter.Core
{
    public class ConverterService : IConverterService
    {
        /// <summary>
        /// Сompiles the exchange rate for all currencies
        /// </summary>
        /// <param name="rate">Currency by which get rate all currencies</param>
        /// <returns>List currencies with price by all currencies include param "rate"</returns>
        public List<ConvertedRateDto> GetRateAllCurrencies(IList<SavedCurrencyDto> currencies)
        {
            var result = new List<ConvertedRateDto>(currencies.Count);

            foreach (var currentCurrency in currencies)
            {
                var newCurrency = new ConvertedRateDto
                {
                    CurrencyId = currentCurrency.CurrencyId
                };
                
                foreach (var otherCurrency in currencies)
                {
                    if (otherCurrency.CurrencyId == currentCurrency.CurrencyId 
                        || otherCurrency.Price == 0) 
                        continue;
                    var currencyRate = currentCurrency.Price / otherCurrency.Price;
                    newCurrency.Rates.Add(otherCurrency.CurrencyId, currencyRate);
                }
                result.Add(newCurrency);
            }

            return result;
        }
    }
}
