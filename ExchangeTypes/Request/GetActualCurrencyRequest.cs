
using System;

namespace ExchangeTypes.Request
{
    public record GetActualCurrencyRequest
    {
        public Guid CorrelationId { get; set; }
    }
}
