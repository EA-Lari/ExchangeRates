using System;

namespace ExchangeTypes.DTO
{
    public class CurrencyRateDto
    {
        public int Id { get; set; }

        public int BaseCurrencyId { get; set; }

        public int CurrencyId { get; set; }

        public DateTime Date { get; set; }

        public decimal Value { get; set; }
    }
}
