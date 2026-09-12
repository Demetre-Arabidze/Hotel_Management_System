using HMS.Application.Contracts.Services;
using HMS.Application.Models.Room;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS.API.Controllers
{
    [ApiController]
    [Route("api/hotels/{hotelId:guid}/rooms")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoomsForHotel(Guid hotelId, [FromQuery] RoomSearchFilterDto filter)
        {
            var rooms = await _roomService.GetHotelRoomsAsync(hotelId, filter);
            return Ok(rooms);
        }

        [HttpGet("{roomId:guid}")]
        public async Task<IActionResult> GetRoomById(Guid hotelId, Guid roomId)
        {
            var room = await _roomService.GetByIdAsync(hotelId, roomId);
            return Ok(room);
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateRoom(Guid hotelId, [FromBody] RoomCreateUpdateDto createDto)
        {
            var createdRoom = await _roomService.CreateAsync(hotelId, createDto);

            return CreatedAtAction(nameof(GetRoomById), new { hotelId = hotelId, roomId = createdRoom.Id }, createdRoom);
        }

        [HttpPut("{roomId:guid}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateRoom(Guid hotelId, Guid roomId, [FromBody] RoomCreateUpdateDto updateDto)
        {
            await _roomService.UpdateAsync(hotelId, roomId, updateDto);
            return NoContent();
        }

        [HttpDelete("{roomId:guid}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteRoom(Guid hotelId, Guid roomId)
        {
            await _roomService.DeleteAsync(hotelId, roomId);
            return NoContent();
        }
    }
}
