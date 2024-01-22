using System.Net.Http.Json;
using CurrencyExchangeService.Business.DTOs;
using Moq;
using CurrencyExchangeService.Business.Services;
using CurrencyExchangeService.Data.Repositories;
using Microsoft.Extensions.Caching.Memory;
using CurrencyExchangeService.Business.Models;
using System.Net;
using Microsoft.Extensions.Options;

namespace CurrencyExchangeService.Tests.BusinessTests
{
    public class ExchangeRateServiceTests
    {
        private readonly Mock<ICurrencyExchangeTradeRepository> _mockRepository;
        private readonly MemoryCache _memoryCache;
        private readonly TestHttpMessageHandler _testHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly ExchangeRateService _exchangeRateService;
        private readonly ExchangeRateServiceConfig _config;

        public ExchangeRateServiceTests()
        {
            _mockRepository = new Mock<ICurrencyExchangeTradeRepository>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _testHttpMessageHandler = new TestHttpMessageHandler();
            _httpClient = new HttpClient(_testHttpMessageHandler);
            _config = new ExchangeRateServiceConfig
            {
                ApiBaseUrl = "http://api.exchangeratesapi.io/v1/latest",
                AccessKey = "a06a5f2a1bf882b0012ada8b88d7a1fa",
                CacheDurationMinutes = 30
            };

            _exchangeRateService = new ExchangeRateService(
                _httpClient,
                _mockRepository.Object,
                _memoryCache,
                Options.Create(_config));

            SetupMemoryCache();
        }

        private void SetupMemoryCache()
        {
            var fakeRateResponse = new ExchangeRateResponse
            {
                Success = true,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Base = "USD",
                Rates = new Dictionary<string, decimal> { { "EUR", 0.85m } },
                Date = DateTime.UtcNow
            };

            var cacheKey = $"ExchangeRates_USD_EUR";
            _memoryCache.Set(cacheKey, fakeRateResponse, DateTimeOffset.Now.AddMinutes(_config.CacheDurationMinutes));
        }

        private void SetupMemoryCacheWithRates(string baseCurrency, string targetCurrency, decimal rate)
        {
            var fakeRateResponse = new ExchangeRateResponse
            {
                Success = true,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Base = baseCurrency,
                Rates = new Dictionary<string, decimal> { { targetCurrency, rate } },
                Date = DateTime.UtcNow
            };

            var cacheKey = $"ExchangeRates_{baseCurrency}_{targetCurrency}";
            _memoryCache.Set(cacheKey, fakeRateResponse, DateTimeOffset.Now.AddMinutes(_config.CacheDurationMinutes));
        }

        [Fact]
        public async Task GetExchangeAndRecordTrade_WithValidData_ReturnsTradeResponse()
        {
            // Arrange
            var fakeTradeRequest = new TradeRequestDto
            {
                ClientId = "123",
                BaseCurrency = "USD",
                TargetCurrency = "EUR",
                Amount = 100
            };

            _testHttpMessageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new ExchangeRateResponse
                {
                    Success = true,
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    Base = "USD",
                    Rates = new Dictionary<string, decimal> { { "EUR", 0.85m } }
                })
            };

            _mockRepository.Setup(repo => repo.GetRecentTradesCount(It.IsAny<string>(), It.IsAny<TimeSpan>()))
                           .ReturnsAsync(0); 

            // Act
            var result = await _exchangeRateService.GetExchangeAndRecordTrade(fakeTradeRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("USD", result.BaseCurrency);
            Assert.Equal("EUR", result.TargetCurrency);
            Assert.Contains("EUR", result.Rate); 
            Assert.Equal(100, result.Amount);
        }

        [Fact]
        public async Task GetLatestRatesAsync_ReturnsCorrectData()
        {
            // Arrange
            SetupMemoryCacheWithRates("EUR", "USD", 1.1867m);
            _testHttpMessageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"success\":true,\"timestamp\":1627579028,\"base\":\"EUR\",\"date\":\"2021-07-29\",\"rates\":{\"USD\":1.1867}}")
            };

            // Act
            var result = await _exchangeRateService.GetLatestRatesAsync("EUR", new[] { "USD" });

            // Assert
            Assert.True(result.Success);
            Assert.Equal("EUR", result.Base);
            Assert.Equal(1.1867m, result.Rates["USD"]);
        }

    }

}
