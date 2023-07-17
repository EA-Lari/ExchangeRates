using System;
using System.Collections.Generic;

namespace ExchangeTypes.Events
{
    public class UpdateCurrencyRateEvent
    {
        public DateTime Date { get; set; }

        public HashSet<ConvertCurrencyRateEvent> Currencies { get; set; }
    }
}
