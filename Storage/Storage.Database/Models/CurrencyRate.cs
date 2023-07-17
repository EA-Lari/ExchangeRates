using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Storage.Database.Models
{
    public class CurrencyRate
    {
        public int Id { get; set; }

        public int BaseCurrencyId { get; set; }

        public int CurrencyId { get; set; }

        public DateTime Date { get; set; }

        public decimal Value { get; set; }

        public Currency BaseCurrency { get; set; }

        public Currency Currency { get; set; }
    }

    class CurrencyRateConfiguration : IEntityTypeConfiguration<CurrencyRate>
    {
        public void Configure(EntityTypeBuilder<CurrencyRate> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.BaseCurrency)
                .WithMany()
                .HasForeignKey(x => x.BaseCurrencyId);
            builder.HasOne(x => x.Currency)
                .WithMany()
                .HasForeignKey(x => x.CurrencyId);
        }
    }
}
