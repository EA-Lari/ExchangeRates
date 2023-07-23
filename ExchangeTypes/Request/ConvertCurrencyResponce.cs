using Converter.Core;
using System;
using System.Collections.Generic;

namespace ExchangeTypes.Request
{
    public record ConvertCurrencyResponce
    {
        public Guid CorrelationId { get; set; }
        public IList<ConvertedRateDto> Currencies { get; set; }
    }
}
