using HMS.Application.Models.Room;

namespace HMS.Application.Contracts.Services
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomResponseDto>> GetHotelRoomsAsync(Guid hotelId, RoomSearchFilterDto filter);
        Task<RoomResponseDto> GetByIdAsync(Guid hotelId, Guid roomId);
        Task<RoomResponseDto> CreateAsync(Guid hotelId, RoomCreateUpdateDto dto);
        Task UpdateAsync(Guid hotelId, Guid roomId, RoomCreateUpdateDto dto);
        Task DeleteAsync(Guid hotelId, Guid roomId);
    }
}
