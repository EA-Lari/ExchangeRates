using Converter.Core;
using ExchangeTypes.DTO;
using System.Collections.Generic;

namespace ExchangeTypes.Request
{
    public record ConvertCurrencyResponce
    {
        public int CorrelationId { get; set; }
        public List<RateCurrency> Currencies { get; set; }
    }
}
