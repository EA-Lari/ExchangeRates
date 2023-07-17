using System;
using System.Collections.Generic;

namespace Storage.Core.DTO
{
    public class FilterByCurrencyDTO
    {
        public DateTime? DateBegin { get; set; }

        public DateTime? DateEnd { get; set; }

        public List<string> CurrencyCods { get; set; }
    }
}
