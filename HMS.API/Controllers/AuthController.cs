using HMS.Application.Contracts.Services;
using HMS.Application.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(result);
        }

        [HttpPost("register/guest")]
        public async Task<ActionResult<AuthResponseDto>> RegisterGuest([FromBody] RegisterGuestDto dto)
        {
            var result = await _authService.RegisterGuestAsync(dto);
            return Ok(result);
        }

        [HttpPost("register/manager")]
        public async Task<ActionResult<AuthResponseDto>> RegisterManager([FromBody] RegisterManagerDto dto)
        {
            var result = await _authService.RegisterManagerAsync(dto);
            return Ok(result);
        }

        [HttpPost("register/admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminDto dto)
        {
            var response = await _authService.RegisterAdminAsync(dto);
            return Ok(response);
        }
    }
}
