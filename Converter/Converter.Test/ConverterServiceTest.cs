using Converter.Core;
using ExchangeTypes.DTO;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Converter.Test
{
    public class ConverterServiceTest
    {
        [Theory]
        [InlineData(1, 74.8327)]
        [InlineData(2, 84.1718)]
        public void CheckRateByRubTest(int currencyId, decimal value)
        {
            var converter = new ConverterService();
            var rate1 = new SavedCurrencyDto
            {
                CurrencyId = currencyId,
                Price = value
            };
            var rate2 = new SavedCurrencyDto
            {
                CurrencyId = 10,
                Price = 1
            };

            var list = new SavedCurrencyDto[] { rate1, rate2 };
            var result = converter.GetRateAllCurrencies(list);

            var otherCurrency = result.FirstOrDefault(x => x.CurrencyId == currencyId);

            //Check rate by Rub
            Assert.Equal(otherCurrency.Rates[10], value);
            //Should not contain the current currency
            Assert.False(otherCurrency.Rates.ContainsKey(currencyId));
        }

        [Theory]
        [InlineData(1, 74.8327, 2, 84.1718)]
        public void CheckingCalculationOfCurrencies(int charCode1, decimal value1, int charCode2, decimal value2)
        {
            var converter = new ConverterService();
            var rub = new ConvertedRateDto
            {
                CurrencyId = 10,
                Rates = new Dictionary<int, decimal>
                {
                    { charCode1, value1 },
                    { charCode2, value2 }
                }
            };
            var rate1 = new SavedCurrencyDto
            {
                CurrencyId = charCode1,
                Price = value1
            };
            var rate2 = new SavedCurrencyDto
            {
                CurrencyId = charCode2,
                Price = value2
            };
            var list = new SavedCurrencyDto[] { rate1, rate2 };
            var result = converter.GetRateAllCurrencies(list);

            var curr1 = result.FirstOrDefault(x => x.CurrencyId == charCode1);
            var curr2 = result.FirstOrDefault(x => x.CurrencyId == charCode2);

            //Check rate Eur/Usd and Usd/Eur
            Assert.Equal(curr1.Rates[charCode2], rub.Rates[charCode1] / rub.Rates[charCode2]);
            Assert.Equal(curr2.Rates[charCode1], rub.Rates[charCode2] / rub.Rates[charCode1]);
        }
    }
}
