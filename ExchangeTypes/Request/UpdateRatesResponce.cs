using System;

namespace ExchangeTypes.Request
{
    public record UpdateRatesResponce
    {
        public Guid? CorrelationId { get; set; }
    }
}
