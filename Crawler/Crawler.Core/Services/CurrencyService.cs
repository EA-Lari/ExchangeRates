using AutoMapper;
using Crawler.Core.Interfaces;
using ExchangeTypes;
using ExchangeTypes.DTO;
using ExchangeTypes.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crawler.Core
{
    /// <summary>
    /// Provides information about currencies
    /// </summary>
    public class CurrencyService : ICurrencyService
    {
        private readonly ICrawlerClientService _clientService;
        private readonly IMapper _mapper;
        private readonly ILogger<CurrencyService> _logger;

        public CurrencyService(ICrawlerClientService clientService, 
            IMapper mapper, 
            ILogger<CurrencyService> logger)
        {
            _clientService = clientService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get actual info by currencies from CBR direct request
        /// </summary>
        /// <returns>List actual info by currencies</returns>
        /// использовать этот метод для получения всей информации по валютам
        public async Task<List<ActualCurrencyFromWebDto>> GetCurrencyFromCBR()
        {
            var taskInfo = _clientService.GetCurrencyInfos();
            var taskRate = _clientService.GetCurrencyRate(DateTime.Now);

            var infos = await taskInfo;
            var rates = await taskRate;

            var currencies = new List<ActualCurrencyFromWebDto>();
            if (infos == null)
                return currencies;
            foreach (var info in infos)
            {
                var currenncy = _mapper.Map<ActualCurrencyFromWebDto>(info);
                currencies.Add(currenncy);
                var rate = rates.FirstOrDefault(x => x.CharCode == info.IsoCharCode);
                if (rate != null)
                {
                    var value = Convert.ToDecimal(rate.Value);
                    currenncy.Price = value;
                }
            }

            return currencies;
        }
    }
}
