using HMS.Application.Contracts.Services;
using HMS.Application.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [Authorize] // Only logged-in users can log out
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Extract the UserId from the JWT token claims
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(userIdString, out Guid userId))
            {
                await _authService.LogoutAsync(userId);
                return Ok(new { message = "Successfully logged out. Refresh token revoked." });
            }

            return BadRequest("Invalid user token.");
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string token)
        {
            await _authService.ConfirmEmailAsync(userId, token);
            return Ok(new { message = "Email confirmed successfully! You can now log in." });
        }
    }
}
