using CurrencyExchangeService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchangeService.Data.DBContext
{
    public class ExchangeContext : DbContext
    {
        public ExchangeContext(DbContextOptions<ExchangeContext> options)
            : base(options)
        {
        }

        public DbSet<CurrencyExchangeTrade> CurrencyExchangeTrades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CurrencyExchangeTrade>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ClientId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TradeDate).IsRequired();
                entity.Property(e => e.BaseCurrency).IsRequired().HasMaxLength(3);
                entity.Property(e => e.TargetCurrency).IsRequired();
                entity.Property(e => e.Rate).IsRequired();
                entity.Property(e => e.Amount).IsRequired();
            });
        }
    }
}
