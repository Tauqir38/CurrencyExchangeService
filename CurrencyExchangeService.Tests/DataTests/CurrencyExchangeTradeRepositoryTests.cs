using CurrencyExchangeService.Data.DBContext;
using CurrencyExchangeService.Data.Models;
using CurrencyExchangeService.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchangeService.Tests.DataTests
{
    public class CurrencyExchangeTradeRepositoryTests
    {
        private readonly ExchangeContext _exchangeContext;
        private readonly CurrencyExchangeTradeRepository _repository;

        public CurrencyExchangeTradeRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ExchangeContext>()
                .UseInMemoryDatabase(databaseName: "TestCurrencyExchangeDb")
                .Options;
            _exchangeContext = new ExchangeContext(options);
            _repository = new CurrencyExchangeTradeRepository(_exchangeContext);
        }

        [Fact]
        public async Task AddTradeAsync_AddsTradeToDatabase()
        {
            // Arrange
            var trade = new CurrencyExchangeTrade
            {
                ClientId = "client123",
                TradeDate = DateTime.UtcNow,
                BaseCurrency = "USD",
                TargetCurrency = "EUR",
                Amount = 100m,
                Rate = "0.85"
            };

            // Act
            await _repository.AddTradeAsync(trade);

            // Assert
            var addedTrade = await _exchangeContext.CurrencyExchangeTrades.FindAsync(trade.Id);
            Assert.NotNull(addedTrade);
            Assert.Equal(trade.ClientId, addedTrade.ClientId);
            Assert.Equal(trade.TradeDate, addedTrade.TradeDate);
            Assert.Equal(trade.BaseCurrency, addedTrade.BaseCurrency);
            Assert.Equal(trade.TargetCurrency, addedTrade.TargetCurrency);
            Assert.Equal(trade.Amount, addedTrade.Amount);
            Assert.Equal(trade.Rate, addedTrade.Rate);
        }
    }
}
