using ExchangeTypes.DTO;
using System.Collections.Generic;

namespace Converter.Core
{
    /// <summary>
    /// Сompiles the exchange rate for all currencies
    /// </summary>
    /// <param name="rate">Currency by which get rate all currencies</param>
    /// <returns>List currencies with price by all currencies include param "rate"</returns>
    public interface IConverterService
    {
        List<ConvertedRateDto> GetRateAllCurrencies(IList<SavedCurrencyDto> currencies);
    }
}
