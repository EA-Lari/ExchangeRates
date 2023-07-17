using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter.Core
{
    /// <summary>
    /// Currency info
    /// </summary>
    public class RateCurrency
    {
        /// <summary>
        /// Name current currency
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Currencies prices in current currency 
        /// </summary>
        public Dictionary<string, decimal> Prices { get; set; }
            = new Dictionary<string, decimal>();
    }
}
