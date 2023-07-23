using System;
using Xunit;
using Crawler.Core;
using Moq;
using System.Threading.Tasks;
using Crawler.Core.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using FluentAssertions;
using Castle.Core.Logging;
using ExchangeTypes.DTO;

namespace Crawler.Test
{
    public class CurrencyServiceTest
    {
        private readonly CurrencyService _service;
        private readonly Mock<ICrawlerClientService> _mockClient;
        private readonly Mock<ICurrencyRepository> _mockRepository;
        private readonly Mock<ILogger> _logger;

        public CurrencyServiceTest()
        {
            _mockClient = new Mock<ICrawlerClientService>();
            _mockRepository = new Mock<ICurrencyRepository>();
            _logger = new Mock<ILogger>();
            //auto mapper configuration
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new CurrancyProfile());
            });
            var mapper = mockMapper.CreateMapper();
        }

        [Fact]
        public async Task CheckConvertModels()
        {
            _mockClient.Setup(x => x.GetCurrencyInfos())
               .Returns(Task.FromResult(new Core.DTO.CurrencyItemInfoDTO[]
                   {
                        new Core.DTO.CurrencyItemInfoDTO
                        {
                            IsoCharCode = "test",
                            EngName = "test name",
                            Nominal = 1,
                            Name = "тест"
                        }
                   }));
            _mockClient.Setup(x => x.GetCurrencyRate(It.IsAny<DateTime>()))
                .Returns(Task.FromResult(new Core.DTO.CurrencyValueDTO[]
                {
                    new Core.DTO.CurrencyValueDTO
                    {
                        CharCode = "test",
                        Value = "12,2222"
                    }
                }));

            var result = await _service.GetCurrencyFromCBR();
            var etalon = new List<ActualCurrencyFromWebDto>
            {
                new ActualCurrencyFromWebDto
                {
                    IsoCode = "test",
                    Price = 12.2222m,
                    EngName = "test name",
                    Nominal = 1,
                    Name = "тест"
                }
            };
            result.Should().BeEquivalentTo(etalon);
        }
    }
}
