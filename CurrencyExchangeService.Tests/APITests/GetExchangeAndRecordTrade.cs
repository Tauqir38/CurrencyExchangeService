using CurrencyExchangeService.API.Controllers;
using CurrencyExchangeService.Business.DTOs;
using CurrencyExchangeService.Business.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace CurrencyExchangeService.Tests.APITests
{
    public class CurrencyExchangeControllerTests
    {
        private readonly Mock<IExchangeRateService> _mockExchangeRateService;
        private readonly Mock<ILogger<CurrencyExchangeController>> _mockLogger;
        private readonly CurrencyExchangeController _controller;

        public CurrencyExchangeControllerTests()
        {
            _mockExchangeRateService = new Mock<IExchangeRateService>();
            _mockLogger = new Mock<ILogger<CurrencyExchangeController>>();
            _controller = new CurrencyExchangeController(_mockExchangeRateService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetExchangeAndRecordTrade_ReturnsOkResult_WithValidTradeRequest()
        {
            // Arrange
            var tradeRequest = new TradeRequestDto
            {
                ClientId = "client123",
                BaseCurrency = "USD",
                TargetCurrency = "EUR",
                Amount = 100m
            };

            var tradeResponse = new TradeResponseDto
            {
                TradeDate = DateTime.UtcNow.ToString(),
                BaseCurrency = "USD",
                TargetCurrency = "EUR",
                Rate = "EUR: 85",
                Amount = 100m
            };

            _mockExchangeRateService.Setup(s => s.GetExchangeAndRecordTrade(It.IsAny<TradeRequestDto>()))
                                    .ReturnsAsync(tradeResponse);

            // Act
            var result = await _controller.GetExchangeAndRecordTrade(tradeRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<TradeResponseDto>(okResult.Value);
            Assert.Equal(tradeResponse.TradeDate, returnValue.TradeDate);
            Assert.Equal(tradeResponse.BaseCurrency, returnValue.BaseCurrency);
            Assert.Equal(tradeResponse.TargetCurrency, returnValue.TargetCurrency);
            Assert.Equal(tradeResponse.Rate, returnValue.Rate);
            Assert.Equal(tradeResponse.Amount, returnValue.Amount);
        }

        [Fact]
        public async Task GetExchangeAndRecordTrade_ReturnsBadRequest_WithInvalidTradeRequest()
        {
            // Arrange
            var invalidTradeRequest = new TradeRequestDto
            {
                ClientId = "",
                BaseCurrency = "US", 
                TargetCurrency = "EUR",
                Amount = 0 
            };

            _controller.ModelState.AddModelError("ClientId", "Required");
            _controller.ModelState.AddModelError("BaseCurrency", "Length must be 3 characters");
            _controller.ModelState.AddModelError("Amount", "Amount must be greater than zero");

            // Act
            var result = await _controller.GetExchangeAndRecordTrade(invalidTradeRequest);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

    }
}
