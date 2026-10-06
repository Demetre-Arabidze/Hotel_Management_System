using HMS.Application.Contracts.Services;
using HMS.Application.Models.Hotel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelsController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllHotels()
        {
            var hotels = await _hotelService.GetAllAsync();
            return Ok(hotels);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetHotelById(Guid id)
        {
            var hotel = await _hotelService.GetByIdWithDetailsAsync(id);
            if (hotel == null) return NotFound("Hotel not found.");

            return Ok(hotel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // FIXED: Only Admin can create
        public async Task<IActionResult> CreateHotel([FromBody] HotelCreateUpdateDto createDto)
        {
            var createdHotel = await _hotelService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetHotelById), new { id = createdHotel.Id }, createdHotel);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,Manager")] // FIXED: Admin or Manager
        public async Task<IActionResult> UpdateHotel(Guid id, [FromBody] HotelCreateUpdateDto updateDto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized("Invalid user token.");

            bool isAdmin = User.IsInRole("Admin");

            // Pass the identity down to the service for domain-level security
            await _hotelService.UpdateAsync(id, updateDto, userId, isAdmin);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")] // FIXED: Only Admin can delete
        public async Task<IActionResult> DeleteHotel(Guid id)
        {
            await _hotelService.DeleteAsync(id);
            return NoContent();
        }
    }
}
