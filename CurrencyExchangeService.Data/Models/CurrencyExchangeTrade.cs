namespace CurrencyExchangeService.Data.Models
{
    public class CurrencyExchangeTrade
    {
        public int Id { get; set; }

        public string ClientId { get; set; }

        public DateTime TradeDate { get; set; }

        public string BaseCurrency { get; set; }

        public string TargetCurrency { get; set; }

        public string Rate { get; set; }

        public decimal Amount { get; set; }
    }
}
