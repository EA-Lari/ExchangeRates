using Crawler.Database.Models;
using ExchangeTypes.Enums;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Crawler.Database
{
    public class OperationPointContext : DbContext
    {
        public DbSet<OperationPoint> OperationPoints{ get; set; }

        public OperationPointContext(DbContextOptions<OperationPointContext> options)
           : base(options)
        {
            NpgsqlConnection.GlobalTypeMapper.MapEnum<OperationTypeEnum>();
        }
    }
}
