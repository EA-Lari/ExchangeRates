using Crawler.Core.DTO;
using ExchangeTypes.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crawler.Core.Interfaces
{
    /// <summary>
    /// Repository for update and read currencies rate
    /// </summary>
    public interface ICurrencyRepository    
    {
        Task UpdateInfos(IEnumerable<CurrencyItemInfoDTO> currencyInfos);

        Task UpdateRates(IEnumerable<CurrencyValueDTO> currencyValues);

        Task<List<ActualCurrencyFromWebDto>> GetTodayCurrencies();

        Task<bool> AnyInfo();

        Task<bool> AnyRateToday();
    }
}
