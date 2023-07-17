using Crawler.Core.DTO;
using Crawler.Core.Interfaces;
using Crawler.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Crawler.Core
{
    public class HttpClientService : ICrawlerClientService
    {
        private readonly HttpClient _httpClient;
        private readonly UrlCurrency _options;
        private readonly ILogger<HttpClientService> _logger;

        public HttpClientService(HttpClient httpClient, 
            IOptions<UrlCurrency> options,
            ILogger<HttpClientService> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<CurrencyItemInfoDTO[]> GetCurrencyInfos()
        {
            _logger.LogInformation("Get info by currencies");
            using (var xml = await _httpClient.GetStreamAsync(_options.Info))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CurrencyInfoDTO));
                var result = (CurrencyInfoDTO)serializer.Deserialize(xml);
                return result.Items;
            }
        }

        public async Task<CurrencyValueDTO[]> GetCurrencyRate(DateTime date)
        {
            _logger.LogInformation("Get currency rates");
            var day = date.ToString("dd/MM/yyyy");
            using (var xml = await _httpClient.GetStreamAsync(_options.Rate + day))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CurrencyRateDTO));
                var result = (CurrencyRateDTO)serializer.Deserialize(xml);
                return result.Values;
            }
        }
    }
}
