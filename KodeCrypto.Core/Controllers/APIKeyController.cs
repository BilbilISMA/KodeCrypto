using KodeCrypto.Application.UseCases.APIKey.Commands;
using Microsoft.AspNetCore.Mvc;

namespace KodeCrypto.Core.Controllers
{
    [Route("api/apikey")]
    [ApiController]

    public class APIKeyController :  BaseController
	{
		public APIKeyController() 
		{
		}

        [HttpPost]
        public async Task<IActionResult> CreateNewAPIKey([FromBody] SaveApiKeyCommand command)
        {
            try
            {
                var request = await Mediator.Send(command);
                return Ok(request);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

    }
}

