
namespace ExchangeTypes.Events
{
    public class ConvertCurrencyRateEvent
    {
        public string IsoCharCode { get; set; }

        public PriceInCurrencyData[] Prices { get; set; }
    }
}
