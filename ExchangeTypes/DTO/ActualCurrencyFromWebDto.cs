namespace ExchangeTypes.DTO
{
    /// <summary>
    /// Currency info from web
    /// </summary>
    public record ActualCurrencyFromWebDto
    {
        public string Name { get; set; }

        public string EngName { get; set; }

        public string IsoCode { get; set; }

        public int Nominal { get; set; }

        public decimal Price { get; set; }
    }
}
