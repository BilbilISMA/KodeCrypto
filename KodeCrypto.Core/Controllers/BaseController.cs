using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KodeCrypto.Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IMediator Mediator => HttpContext.RequestServices.GetService<IMediator>();
    }
}

