using ExchangeTypes.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crawler.Core
{
    public interface ICurrencyService
    {
        Task<List<ActualCurrencyFromWebDto>> GetCurrencyFromCBR();
    }
}