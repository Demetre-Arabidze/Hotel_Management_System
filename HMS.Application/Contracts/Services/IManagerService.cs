using HMS.Application.Models.Manager;

namespace HMS.Application.Contracts.Services
{
    public interface IManagerService
    {
        Task<IEnumerable<ManagerResponseDto>> GetHotelManagersAsync(Guid hotelId);
        Task<ManagerResponseDto> AssignToHotelAsync(Guid hotelId, ManagerCreateUpdateDto dto);
        Task UpdateAsync(Guid hotelId, Guid managerId, ManagerCreateUpdateDto dto);
        Task DeleteAsync(Guid hotelId, Guid managerId);
    }
}
