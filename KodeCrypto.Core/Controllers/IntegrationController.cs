using KodeCrypto.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KodeCrypto.Core.Controllers
{
    //This controller is for testing the integration only. In real-life scenario it shouldn't be exposed.
    [Route("api/integration")]
    [ApiController]
    public class IntegrationController : BaseController
    {
        private readonly IBinanceService _binanceService;
        private readonly IKrakenService _krakenService;

        public IntegrationController(IBinanceService binanceService, IKrakenService krakenService)
        {
            _binanceService = binanceService;
            _krakenService = krakenService;
        }

        [HttpPost("binance-balance")]
        public async Task<IActionResult> SyncBinanceBalance()
        {
            try
            {
                var balance = await _binanceService.GetBalanceAsync();
                return Ok(balance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("binance-trade")]
        public async Task<IActionResult> SyncBinanceTradeHistory()
        {
            try
            {
                var balance = await _binanceService.GetTransactionHistoryAsync();
                return Ok(balance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("kraken-balance")]
        public async Task<IActionResult> SyncKrakenBalance()
        {
            try
            {
                var balance = await _krakenService.GetBalanceAsync();
                return Ok(balance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("kraken-trade")]
        public async Task<IActionResult> SyncKrakenTradeHistory()
        {
            try
            {
                var balance = await _krakenService.GetTransactionHistoryAsync();
                return Ok(balance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}

