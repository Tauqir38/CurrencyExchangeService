using CurrencyExchangeService.Business.DTOs;
using CurrencyExchangeService.Business.Models;

namespace CurrencyExchangeService.Business.Services
{
    public interface IExchangeRateService
    {
        Task<ExchangeRateResponse> GetLatestRatesAsync(string baseCurrency, string[] symbols);
        Task<TradeResponseDto> GetExchangeAndRecordTrade(TradeRequestDto trade);
    }
}
