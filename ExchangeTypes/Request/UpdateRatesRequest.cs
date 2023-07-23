using Converter.Core;
using ExchangeTypes.DTO;
using System;
using System.Collections.Generic;

namespace ExchangeTypes.Request
{
    public record UpdateRatesRequest
    {
        public Guid CorrelationId { get; set; }
        public IList<ConvertedRateDto> Currencies { get; set; }
    }
}
