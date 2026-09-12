using HMS.Application.Contracts.Services;
using HMS.Application.Models.Reservation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers
{
    [ApiController]
    [Route("api/hotels/{hotelId:guid}/[controller]")]
    [Authorize]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpPost]
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> CreateReservation(Guid hotelId, [FromBody] ReservationCreateDto dto)
        {
            var reservationId = await _reservationService.CreateAsync(hotelId, dto);
            return Ok(new { Id = reservationId });
        }

        [HttpPut("{reservationId:guid}/dates")]
        public async Task<IActionResult> UpdateDates(Guid hotelId, Guid reservationId, [FromBody] ReservationUpdateDatesDto dto)
        {
            await _reservationService.UpdateDatesAsync(reservationId, dto);
            return NoContent();
        }

        [HttpDelete("{reservationId:guid}")]
        public async Task<IActionResult> CancelReservation(Guid hotelId, Guid reservationId)
        {
            await _reservationService.CancelAsync(reservationId);
            return NoContent();
        }
    }
}
