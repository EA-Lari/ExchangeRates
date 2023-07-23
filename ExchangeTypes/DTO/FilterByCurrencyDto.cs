using System;
using System.Collections.Generic;

namespace ExchangeTypes.DTO
{
    public class FilterByCurrencyDto
    {
        public DateTime? DateBegin { get; set; }

        public DateTime? DateEnd { get; set; }

        public List<string> CurrencyCods { get; set; }
    }
}
