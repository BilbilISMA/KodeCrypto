using System.Net;
using API.Infrastructure;
using KodeCrypto.Application.DTO.Portfolio;
using KodeCrypto.Application.UseCases.Portfolio.Commands;
using KodeCrypto.Application.UseCases.Portfolio.Queries;
using Microsoft.AspNetCore.Mvc;

namespace KodeCrypto.Core.Controllers
{
    [Route("api/portfolio")]
    [Authorize("Admin")]
    [ApiController]
    public class PortfolioController : BaseController
    {
        [HttpGet("account-balance")]
        [ProducesResponseType(((int)HttpStatusCode.OK), Type = typeof(IEnumerable<AccountBalanceDTO>))]
        public async Task<IActionResult> GetAccountBalance([FromQuery] GetAccountBalanceQuery query)
        {
            try
            {
                var result = await Mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("trade-history")]
        [ProducesResponseType(((int)HttpStatusCode.OK), Type = typeof(IEnumerable<TradeHistoryDTO>))]
        public async Task<IActionResult> GetTradeHistory([FromQuery] GetTradeHistoryQuery query)
        {
            try
            {
                var result = await Mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost("order")]
        [ProducesResponseType(((int)HttpStatusCode.OK), Type = typeof(bool))]
        public async Task<IActionResult> PostOrder([FromBody] PostOrderCommand command)
        {
            try
            {
                var result = await Mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}

