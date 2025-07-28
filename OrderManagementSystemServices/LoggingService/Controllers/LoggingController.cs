using Microsoft.AspNetCore.Mvc;

namespace LoggingService.Controllers
{
    public class LoggingController : Controller
    {
        public async Task<IActionResult> DummyEndpoint()
        {
            return Ok();
        }
    }
}
