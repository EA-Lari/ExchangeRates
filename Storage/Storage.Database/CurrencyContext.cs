using Microsoft.EntityFrameworkCore;
using Storage.Database.Models;

namespace Storage.Database
{
    public class CurrencyContext : DbContext
    {
        public DbSet<Currency> Currencies { get; set; }

        public DbSet<CurrencyRate> CurrencyRates { get; set; }

        public CurrencyContext(DbContextOptions<CurrencyContext> options)
            : base(options)
        { }
    }
}
