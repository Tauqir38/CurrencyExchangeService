namespace CurrencyExchangeService.Business.Models
{
    public class ExchangeRateServiceConfig
    {
        public string ApiBaseUrl { get; set; }
        public string AccessKey { get; set; }
        public int CacheDurationMinutes { get; set; }
    }
}
