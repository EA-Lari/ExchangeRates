using System;

namespace ExchangeTypes.DTO
{
    public class CurrencyDTO
    {
        public string Name { get; set; }

        public string EngName { get; set; }

        public string CharCode { get; set; }

        public int Nominal { get; set; }

        public DateTime Date { get; set; }

        public decimal? Value { get; set; }
    }
}
