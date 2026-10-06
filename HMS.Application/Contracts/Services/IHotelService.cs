using HMS.Application.Models.Hotel;

namespace HMS.Application.Contracts.Services
{
    public interface IHotelService
    {
        Task<IEnumerable<HotelResponseDto>> GetAllAsync();
        Task<HotelResponseDto> GetByIdAsync(Guid id);
        Task<HotelWithManagerDto> GetByIdWithDetailsAsync(Guid id);
        Task<HotelResponseDto> CreateAsync(HotelCreateUpdateDto dto);
        Task UpdateAsync(Guid id, HotelCreateUpdateDto dto, Guid userId, bool isAdmin);
        Task DeleteAsync(Guid id);
    }
}
