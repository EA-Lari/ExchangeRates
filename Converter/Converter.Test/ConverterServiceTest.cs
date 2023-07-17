using Converter.Core;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Converter.Test
{
    public class ConverterServiceTest
    {
        [Theory]
        [InlineData("Usd", 74.8327)]
        [InlineData("Eur", 84.1718)]
        public void CheckRateByRubTest(string charCode, decimal value)
        {
            var converter = new ConverterService();
            var rate = new RateCurrency
            {
                Name = "Rub",
                Prices = new Dictionary<string, decimal>
                {
                    {  charCode, value }
                }
            };
            var result = converter.GetRateAllCurrencies(rate);

            var otherCurrency = result.FirstOrDefault(x => x.Name == charCode);

            //Check rate by Rub
            Assert.Equal(otherCurrency.Prices["Rub"], value);
            //Should not contain the current currency
            Assert.False(otherCurrency.Prices.ContainsKey(charCode));
        }

        [Theory]
        [InlineData("Usd", 74.8327, "Eur", 84.1718)]
        public void CheckingCalculationOfCurrencies(string charCode1, decimal value1, string charCode2, decimal value2)
        {
            var converter = new ConverterService();
            var rate = new RateCurrency
            {
                Name = "Rub",
                Prices = new Dictionary<string, decimal>
                {
                    { charCode1, value1 },
                    { charCode2, value2 }
                }
            };
            var result = converter.GetRateAllCurrencies(rate);

            var curr1 = result.FirstOrDefault(x => x.Name == charCode1);
            var curr2 = result.FirstOrDefault(x => x.Name == charCode2);
            var rub = result.FirstOrDefault(x => x.Name == "Rub");

            //Check rate Eur/Usd and Usd/Eur
            Assert.Equal(curr1.Prices[charCode2], rub.Prices[charCode1] / rub.Prices[charCode2]);
            Assert.Equal(curr2.Prices[charCode1], rub.Prices[charCode2] / rub.Prices[charCode1]);
        }
    }
}
