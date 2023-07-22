namespace ExchangeTypes.DTO
{
    /// <summary>
    /// Updated currency's id
    /// </summary>
    public record SavedCurrencyDto
    {
        public int CurrencyId { get; set; }

        public decimal Price { get; set; }
    }
}
