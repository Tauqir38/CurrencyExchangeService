namespace CurrencyExchangeService.Business.DTOs
{
    public class TradeResponseDto
    {
        public string TradeDate { get; set; }
        public string BaseCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public string Rate { get; set; }
        public decimal Amount { get; set; }
    }
}
