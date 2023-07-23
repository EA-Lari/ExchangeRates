using AutoMapper;
using AutoMapper.QueryableExtensions;
using Converter.Core;
using ExchangeTypes.DTO;
using ExchangeTypes.Events;
using ExchangeTypes.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Storage.Database;
using Storage.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Core.Repositories
{
    /// <summary>
    /// Operations with currencies in database
    /// </summary>
    public class CurrencyRatesRepository
    {
        private readonly CurrencyContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CurrencyRatesRepository> _logger;

        public CurrencyRatesRepository(CurrencyContext context, IMapper mapper, ILogger<CurrencyRatesRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Selects currency rates by filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>List with rates by filter</returns>
        public Task<List<CurrencyRateDto>> GetCurrencies(FilterByCurrencyDto filter)
        {
            var currencyQuery = _context.CurrencyRates.AsQueryable();
            if (filter.DateBegin != null)
                currencyQuery = currencyQuery
                .Where(x => x.Date >= filter.DateBegin);
            if (filter.DateBegin != null)
                currencyQuery = currencyQuery
                .Where(x => x.Date <= filter.DateEnd);
            if (filter.CurrencyCods != null)
                if (filter.CurrencyCods.Count != 0)
                    currencyQuery = currencyQuery
                        .Where(x => filter.CurrencyCods.Any(isoCode => isoCode == x.Currency.IsoCode));
            return currencyQuery

                .Select(x => _mapper.Map<CurrencyRateDto>(x))
                //.Select(x => new CurrencyRateDto
                //{
                //    BaseCurrencyId = x.BaseCurrencyId,
                //    CurrencyId = x.CurrencyId,
                //    Date = x.Date,
                //    Id = x.Id,
                //    Value = x.Value,
                //})
                .ToListAsync();
        }

        /// <summary>
        /// Convert event model to model and save currency rates
        /// </summary>
        /// <param name="rates">List of currency rates in DTO</param>
        public async Task SaveCurrencyRates(IEnumerable<ConvertCurrencyRateEvent> rates)
        {
            _logger.LogInformation("SAVE RATEs START");
            var currencies = await _context.Currencies.ToListAsync();
            var currencyCurrent = currencies
               .Join(rates, c => c.IsoCode, r => r.IsoCharCode, (c, r) =>
               new
               {
                   Currency = c,
                   Rates = r.Prices.Join(currencies, p => p.IsoCharCode, c => c.IsoCode, (p, c) =>
                   new
                   {
                       BaseID = c.Id,
                       Value = p.Value
                   })
               }).ToList();

            foreach (var cRate in currencyCurrent)
            {
                var nRate = cRate.Rates
                    .Select(x => new CurrencyRate
                    {
                        BaseCurrencyId = x.BaseID,
                        CurrencyId = cRate.Currency.Id,
                        Date = DateTime.Now,
                        Value = x.Value,
                    })
                    .ToList();

                await _context.CurrencyRates.AddRangeAsync(nRate);
            }
            
            await _context.SaveChangesAsync();

            _logger.LogInformation("SAVE RATEs End");
        }

        /// <summary>
        /// Convert event model to model and save currency rates
        /// </summary>
        /// <param name="currencies">List of currency rates in DTO</param>
        public async Task SaveCurrencyRates(IEnumerable<ConvertedRateDto> currencies)
        {
            _logger.LogInformation("SAVE RATEs START");
          
            foreach (var currency in currencies)
            {
                var rates = currency.Rates
                    .Select(x => new CurrencyRate
                    {
                        BaseCurrencyId = x.Key,
                        CurrencyId = currency.CurrencyId,
                        Date = DateTime.Now,
                        Value = x.Value,
                    })
                    .ToList();

                await _context.CurrencyRates.AddRangeAsync(rates);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("SAVE RATEs End");
        }

        /// <summary>
        /// Convert event model to model and save info about currencies
        /// </summary>
        /// <param name="rates">List of info about currency in DTO</param>
        public async Task<IList<SavedCurrencyDto>> SaveCurrencyInfo(IEnumerable<ActualCurrencyFromWebDto> currencyInfos)
        {
            _logger.LogInformation("START SAVE Currency info");

            var withCode = currencyInfos
                .Where(x => !string.IsNullOrEmpty(x.IsoCode))
                .ToList();

            withCode.Add(new ActualCurrencyFromWebDto
            {
                Price = 1,
                IsoCode = "RUS",
            });

            //TODO: need change on AddOrUpdate
            var currencies = await _context.Currencies.ToListAsync();

            var result = new List<SavedCurrencyDto>(currencyInfos.Count());

            foreach (var dto in withCode)
            {
                var currency = currencies.FirstOrDefault(x => x.IsoCode == dto.IsoCode);
                if (currency == null)
                {
                    currency = _mapper.Map<Currency>(dto);
                    await _context.AddAsync(currency);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _mapper.Map(dto, currency);
                    await _context.SaveChangesAsync();
                }

                result.Add(new SavedCurrencyDto
                {
                    CurrencyId = currency.Id,
                    Price = dto.Price
                });
            }

            _logger.LogInformation("END SAVE Currency info");

            return result;
        }
    }
}
