using Crawler.Core.DTO;
using System;
using System.Threading.Tasks;

namespace Crawler.Core.Interfaces
{
    /// <summary>
    /// Load info about currencies rate
    /// </summary>
    public interface ICrawlerClientService
    {
        /// <summary>
        /// All currencies array with base informations
        /// </summary>
        /// <returns></returns>
        Task<CurrencyItemInfoDTO[]> GetCurrencyInfos();

        /// <summary>
        /// Currencies rate array
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        Task<CurrencyValueDTO[]> GetCurrencyRate(DateTime date);
    }
}
