
using MassTransit;
using System;

namespace ExchangeTypes.Events
{
    public class UpdateCurrencyInfoEvent : CorrelatedBy<Guid>
    {
        public CurrencyInfoData[] Currencies { get; set; }

        public Guid CorrelationId { get; set; }
    }
}
