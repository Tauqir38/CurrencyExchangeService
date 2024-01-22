using CurrencyExchangeService.Data.DBContext;
using CurrencyExchangeService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchangeService.Data.Repositories
{
    public class CurrencyExchangeTradeRepository : ICurrencyExchangeTradeRepository
    {
        private readonly ExchangeContext _context;

        public CurrencyExchangeTradeRepository(ExchangeContext context)
        {
            _context = context;
        }

        public async Task AddTradeAsync(CurrencyExchangeTrade trade)
        {
            await _context.CurrencyExchangeTrades.AddAsync(trade);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetRecentTradesCount(string clientId, TimeSpan timeSpan)
        {
            var timeLimit = DateTime.UtcNow.Subtract(timeSpan);
            return await _context.CurrencyExchangeTrades
                                 .Where(trade => trade.ClientId == clientId && trade.TradeDate >= timeLimit)
                                 .CountAsync();
        }
    }
}
