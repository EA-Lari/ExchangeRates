using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Storage.Database.Models
{
    public class Currency
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string EngName { get; set; }

        /// <summary>
        /// Parent Code
        /// </summary>
        public string RId { get; set; }

        public string IsoCode { get; set; }
    }

    class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
