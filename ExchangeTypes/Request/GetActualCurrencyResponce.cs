using ExchangeTypes.DTO;
using System.Collections.Generic;

namespace ExchangeTypes.Request
{
    public record GetActualCurrencyResponce
    {
        public int CorrelationId { get; set; }
        public IList<CurrencyDTO> Currencies { get; set; }
    }
}
