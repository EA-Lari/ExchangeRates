using System.Collections.Generic;

namespace Converter.Core
{
    /// <summary>
    /// Currency info
    /// </summary>
    public record ConvertedRateDto
    {
        /// <summary>
        /// Name current currency
        /// </summary>
        public int CurrencyId { get; set; }

        /// <summary>
        /// Currencies prices in current currency 
        /// </summary>
        public IDictionary<int, decimal> Rates{ get; set; }
            = new Dictionary<int, decimal>();
    }
}
