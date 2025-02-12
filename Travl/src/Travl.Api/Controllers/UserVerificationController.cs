using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Travl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserVerificationController : ControllerBase
    {
        [HttpPost("verify")]
        public async Task<IActionResult> Verify()
        {
            return Ok();
        }
    }
}
