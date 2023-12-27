using KodeCrypto.Application.UseCases.Identity.Commands;
using KodeCrypto.Application.UseCases.Identity.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KodeCrypto.Core.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : BaseController
    {
        /// <summary>
        /// Signup a new user
        /// </summary>
        /// <response code="204">No Content</response> 
        /// <response code="400">Bad Request</response> 
        /// <response code="500">Internal Server Error</response> 
        [AllowAnonymous]
        [HttpPost]
        [Route("signup")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> SignUp([FromBody] SignUpCommand command)
        {
            bool result = await Mediator.Send(command);

            return NoContent();

        }

        /// <summary>
        /// Creates a session, user  login
        /// </summary>
        /// <param name="command"></param>
        /// <response code="402">Bad Request</response> 
        /// <response code="500">Internal Server Error</response> 
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(200, Type = typeof(JsonWebTokenDTO))]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            JsonWebTokenDTO result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}

