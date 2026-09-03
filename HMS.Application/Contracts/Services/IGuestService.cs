using HMS.Application.Models.Guest;

namespace HMS.Application.Contracts.Services
{
    public interface IGuestService
    {
        Task<GuestResponseDto> GetByIdAsync(Guid id);
        Task<GuestResponseDto> CreateAsync(GuestCreateUpdateDto dto);
        Task UpdateAsync(Guid id, GuestCreateUpdateDto dto);
        Task DeleteAsync(Guid id);
    }
}
