using KodeCrypto.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KodeCrypto.Core.Controllers
{
    //This controller is for testing the integration only. In real-life scenario it shouldn't be exposed.
    [Route("api/integration")]
    [ApiController]
    public class IntegrationController : BaseController
    {
        private readonly ISyncService _syncService;

        public IntegrationController(ISyncService syncService)
        {
            _syncService = syncService;
        }

        [HttpPost("binance-balance")]
        public async Task<IActionResult> SyncBinanceBalance()
        {
            try
            {
                var balance = await _syncService.SyncBalance();
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
                var balance = await _syncService.SyncTransactionHistory();
                return Ok(balance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("binance-orders")]
        public async Task<IActionResult> SyncBinanceOrders()
        {
            try
            {
                var balance = await _syncService.SyncOrders();
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
                var balance = await _syncService.SyncBalance();
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
                var balance = await _syncService.SyncTransactionHistory();
                return Ok(balance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("kraken-orders")]
        public async Task<IActionResult> SyncKrakenOrders()
        {
            try
            {
                var balance = await _syncService.SyncOrders();
                return Ok(balance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}

