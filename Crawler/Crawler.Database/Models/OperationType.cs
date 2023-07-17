using ExchangeTypes.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Crawler.Database.Models
{
    public class OperationType
    {
        public OperationTypeEnum Id { get; set; }

        public string Name { get; set; }
    }

    public class OperationTypeConfiguretion : IEntityTypeConfiguration<OperationType>
    {
        public void Configure(EntityTypeBuilder<OperationType> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasColumnType("byte");
        }
    }
}
