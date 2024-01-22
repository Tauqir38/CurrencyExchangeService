using CurrencyExchangeService.Business.DTOs;
using CurrencyExchangeService.Data.Models;

namespace CurrencyExchangeService.Business.Mappers
{
    public static class TradeMapper
    {
        public static CurrencyExchangeTrade ToEntity(this TradeRequestDto request)
        {
            return new CurrencyExchangeTrade
            {
                ClientId = request.ClientId,
                BaseCurrency = request.BaseCurrency,
                TargetCurrency = request.TargetCurrency,
                Amount=request.Amount,
                TradeDate= DateTime.Now
            };
        }

        public static TradeResponseDto ToDto(this CurrencyExchangeTrade trade)
        {
            return new TradeResponseDto
            {
                TradeDate = trade.TradeDate.ToString("yyyy-MM-dd HH:mm:ss"),
                BaseCurrency = trade.BaseCurrency,
                TargetCurrency = trade.TargetCurrency,
                Rate = trade.Rate, 
                Amount= trade.Amount
            };
        }
    }
}
