using CurrencyExchangeService.Business.DTOs;
using CurrencyExchangeService.Business.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CurrencyExchangeService.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyExchangeController : ControllerBase
    {
        private readonly IExchangeRateService _exchangeRateService;
        private readonly ILogger<CurrencyExchangeController> _logger;
        public CurrencyExchangeController(IExchangeRateService exchangeRateService
            , ILogger<CurrencyExchangeController> logger)
        {
            _exchangeRateService = exchangeRateService;
            _logger = logger;
        }

        [HttpPost("currency-exchange")]
        public async Task<IActionResult> GetExchangeAndRecordTrade([FromBody] TradeRequestDto trade)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  
            }

            try
            {
                return Ok(await _exchangeRateService.GetExchangeAndRecordTrade(trade));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Parameter: {JsonConvert.SerializeObject(trade)}, exception: {ex}");
                return BadRequest("There is problem with your request");
            }
        }

    }
}
