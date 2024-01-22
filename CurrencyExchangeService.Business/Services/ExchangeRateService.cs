using CurrencyExchangeService.Business.DTOs;
using CurrencyExchangeService.Business.Mappers;
using CurrencyExchangeService.Business.Models;
using CurrencyExchangeService.Data.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CurrencyExchangeService.Business.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly ICurrencyExchangeTradeRepository _tradeRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly HttpClient _httpClient;
        private readonly ExchangeRateServiceConfig _config;
        private TimeSpan _cacheDuration;

        public ExchangeRateService(HttpClient httpClient, ICurrencyExchangeTradeRepository tradeRepository,
            IMemoryCache memoryCache, IOptions<ExchangeRateServiceConfig> configOptions)
        {
            _tradeRepository = tradeRepository;
            _memoryCache = memoryCache;
            _httpClient = httpClient;
            _config = configOptions.Value;
            _cacheDuration = TimeSpan.FromMinutes(_config.CacheDurationMinutes);
        }

        public async Task<ExchangeRateResponse> GetLatestRatesAsync(string baseCurrency, string[] symbols)
        {
            var cacheKey = $"ExchangeRates_{baseCurrency}_{string.Join("_", symbols)}";
            if (!_memoryCache.TryGetValue(cacheKey, out ExchangeRateResponse cachedRates))
            {
                var response = await _httpClient.GetAsync($"{_config.ApiBaseUrl}?access_key={_config.AccessKey}&base={baseCurrency}&symbols={string.Join(",", symbols)}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                cachedRates = JsonConvert.DeserializeObject<ExchangeRateResponse>(content);

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(_cacheDuration);
                _memoryCache.Set(cacheKey, cachedRates, cacheEntryOptions);
            }

            return cachedRates;
        }
        public async Task<TradeResponseDto> GetExchangeAndRecordTrade(TradeRequestDto trade)
        {
            if (string.IsNullOrWhiteSpace(trade.ClientId))
            {
                throw new ArgumentException("Invalid Client ID.");
            }

            if (string.IsNullOrWhiteSpace(trade.BaseCurrency) || string.IsNullOrWhiteSpace(trade.TargetCurrency))
            {
                throw new ArgumentException("Currency codes must be valid.");
            }

            // Fetch the latest rates
            var latestRates = await GetLatestRatesAsync(trade.BaseCurrency, new[] { trade.TargetCurrency });

            // Check if the rate is fresh
            if (DateTimeOffset.FromUnixTimeSeconds(latestRates.Timestamp).Add(_cacheDuration) < DateTime.UtcNow)
            {
                throw new InvalidOperationException("The exchange rate is older than 30 minutes.");
            }

            // Check trade frequency limit
            var recentTradesCount = await _tradeRepository.GetRecentTradesCount(trade.ClientId, TimeSpan.FromHours(1));
            if (recentTradesCount >= 10)
            {
                throw new InvalidOperationException("Trade limit exceeded. Only 10 trades per hour are allowed.");
            }

            var tradeEnt = TradeMapper.ToEntity(trade);
            tradeEnt.Rate = MuliplyAmountConvertString(latestRates.Rates, trade.Amount);
            await _tradeRepository.AddTradeAsync(tradeEnt);
            return TradeMapper.ToDto(tradeEnt);
        }
        private string MuliplyAmountConvertString(Dictionary<string, decimal> rates, decimal amount)
        {
            return string.Join(", ", rates.Select(kv => $"{kv.Key}: {kv.Value * amount}"));
        }
    }
}
