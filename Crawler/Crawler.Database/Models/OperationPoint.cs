using ExchangeTypes.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Crawler.Database.Models
{
    public class OperationPoint
    {
        public int Id{ get; set; }

        public OperationTypeEnum TypeId { get; set; }

        public OperationType Type { get; set; }

        public DateTime Date{ get; set; }

        public bool IsSuccess { get; set; }
    }

    public class OperationPointConfiguration : IEntityTypeConfiguration<OperationPoint>
    {
        public void Configure(EntityTypeBuilder<OperationPoint> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Type)
                .WithMany()
                .HasForeignKey(x => x.TypeId);
        }
    }
}
