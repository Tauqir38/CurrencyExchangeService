using CurrencyExchangeService.Data.Models;

namespace CurrencyExchangeService.Data.Repositories
{
    public interface ICurrencyExchangeTradeRepository
    {
        Task AddTradeAsync(CurrencyExchangeTrade trade);
        Task<int> GetRecentTradesCount(string clientId, TimeSpan timeSpan);
    }
}
