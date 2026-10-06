using HMS.Domain.Entities;

namespace HMS.Application.Contracts.Persistence
{
    public interface IHotelRepository : IRepositoryBase<Hotel>
    {
        Task<Hotel?> GetByIdWithDetailsAsync(Guid id);
    }
}
