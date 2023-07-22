using ExchangeTypes.DTO;
using System;
using System.Collections.Generic;

namespace ExchangeTypes.Request
{
    public record ConvertCurrencyRequest
    {
        public Guid? CorrelationId { get; set; }
        public IList<SavedCurrencyDto> Currencies { get; set; }
    }
}
