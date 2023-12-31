﻿using ExchangeTypes.DTO;
using System;
using System.Collections.Generic;

namespace ExchangeTypes.Request
{
    public record GetActualCurrencyResponce
    {
        public Guid CorrelationId { get; set; }
        public IList<ActualCurrencyFromWebDto> Currencies { get; set; }
    }
}
