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
    public class CurrencyService
    {
        private readonly ICrawlerClientService _clientService;
        private readonly IMapper _mapper;
        private readonly CurrencyPublisher _publisher;
        private readonly ILogger<CurrencyService> _logger;

        public CurrencyService(ICrawlerClientService clientService, 
            IMapper mapper, 
            CurrencyPublisher publisher,
            ILogger<CurrencyService> logger)
        {
            _clientService = clientService;
            _mapper = mapper;
            _publisher = publisher;
            _logger = logger;
        }

        /// <summary>
        /// Load and publish currencies info
        /// </summary>
        public async Task LoadCurrencyInfos()
        {
            Console.WriteLine("\nUpdate Currencies\n");
            var infoDtos = await _clientService.GetCurrencyInfos();
            var updateInfo = new UpdateCurrencyInfoEvent();
            updateInfo.Currencies = infoDtos
                .Select(x => _mapper.Map<CurrencyInfoData>(x)
                ).ToArray();
            var text = "Publish for update info by currencies";
            _logger.LogInformation(text);
            Console.WriteLine(text);
            await _publisher.Publish(updateInfo);
        }

        /// <summary>
        /// Load and publish currency rates
        /// </summary>
        public async Task LoadCurrencyTodayRates()
        {
            Console.WriteLine("\nUpdate Currency rates\n");
            var values = await _clientService.GetCurrencyRate(DateTime.Now);
            var prices = values
                .Select(x => _mapper.Map<PriceInCurrencyData>(x))
                .ToArray();
            var rates = new ConvertCurrencyRateEvent
            {
                IsoCharCode = "RUB",
                Prices = prices
            };
            var text = "Publish Currency rates";
            _logger.LogInformation(text);
            Console.WriteLine(text);
            await _publisher.Publish(rates);
        }

        /// <summary>
        /// Get actual info by currencies from CBR direct request
        /// </summary>
        /// <returns>List actual info by currencies</returns>
        /// использовать этот метод для получения всей информации по валютам
        public async Task<List<CurrencyDTO>> GetCurrencyFromCBR()
        {
            var taskInfo = _clientService.GetCurrencyInfos();
            var taskRate = _clientService.GetCurrencyRate(DateTime.Now);

            var infos = await taskInfo;
            var rates = await taskRate;

            var currencies = new List<CurrencyDTO>();
            if (infos == null)
                return currencies;
            foreach (var info in infos)
            {
                var currenncy = _mapper.Map<CurrencyDTO>(info);
                currencies.Add(currenncy);
                currenncy.Date = DateTime.Today;
                var rate = rates.FirstOrDefault(x => x.CharCode == info.IsoCharCode);
                if (rate != null)
                {
                    var value = Convert.ToDecimal(rate.Value);
                    currenncy.Value = value;
                }
            }

            return currencies;
        }
    }
}
