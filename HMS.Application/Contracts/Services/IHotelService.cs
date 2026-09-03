using HMS.Application.Models.Hotel;

namespace HMS.Application.Contracts.Services
{
    public interface IHotelService
    {
        Task<IEnumerable<HotelResponseDto>> GetAllAsync(HotelFilterDto filter);
        Task<HotelResponseDto> GetByIdAsync(Guid id);
        Task<HotelResponseDto> CreateAsync(HotelCreateUpdateDto dto);
        Task UpdateAsync(Guid id, HotelCreateUpdateDto dto);
        Task DeleteAsync(Guid id);
    }
}
