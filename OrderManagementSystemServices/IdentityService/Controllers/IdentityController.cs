using Microsoft.AspNetCore.Mvc;
using IdentityService.Services;
using IdentityService.Models;
using Microsoft.AspNetCore.Authorization;

namespace IdentityService.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _identityService.Authenticate(request.Username, request.Password);
            if (response == null)
            {
                return Unauthorized();
            }
            return Ok(response);
        }
    }
}