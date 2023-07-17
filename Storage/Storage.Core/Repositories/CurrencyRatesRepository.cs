﻿using AutoMapper;
using ExchangeTypes.Events;
using Microsoft.EntityFrameworkCore;
using Storage.Core.DTO;
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

        public CurrencyRatesRepository(CurrencyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Selects currency rates by filter
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>List with rates by filter</returns>
        public List<CurrencyRate> GetCurrencies(FilterByCurrencyDTO filter)
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
            return currencyQuery.ToList();
        }

        /// <summary>
        /// Convert event model to model and save currency rates
        /// </summary>
        /// <param name="rates">List of currency rates in DTO</param>
        public async Task SaveCurrencyRates(IEnumerable<ConvertCurrencyRateEvent> rates)
        {
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
            Console.WriteLine("SAVE RATEs START");
            await _context.SaveChangesAsync();
            Console.WriteLine("SAVE RATEs End");
        }

        /// <summary>
        /// Convert event model to model and save info about currencies
        /// </summary>
        /// <param name="rates">List of info about currency in DTO</param>
        public async Task SaveCurrencyInfo(IEnumerable<CurrencyInfoData> currencyInfos)
        {
            var withCode = currencyInfos
                .Where(x => !string.IsNullOrEmpty(x.IsoCode))
                .ToArray();
            var currencies = await _context.Currencies.ToListAsync();
            foreach (var dto in withCode)
            {
                var currency = currencies.FirstOrDefault(x => x.IsoCode == dto.IsoCode);
                if (currency == null)
                {
                    var newCurrency = _mapper.Map<Currency>(dto);
                    await _context.AddAsync(newCurrency);
                }
                else
                {
                    _mapper.Map(dto, currency);
                }
            }
            Console.WriteLine("SAVE info START");
            await _context.SaveChangesAsync();
            Console.WriteLine("SAVE info End");
        }
    }
}
