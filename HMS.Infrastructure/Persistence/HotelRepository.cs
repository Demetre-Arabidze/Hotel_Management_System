using HMS.Application.Contracts.Persistence;
using HMS.Domain.Entities;
using HMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Persistence
{
    public class HotelRepository : RepositoryBase<Hotel>, IHotelRepository
    {
        private readonly ApplicationDbContext _context;

        public HotelRepository(ApplicationDbContext context) : base(context)   
        {
            _context = context;
        }

        public async Task<Hotel?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.Hotels
                .Include(h => h.Managers) 
                .AsNoTracking()           
                .FirstOrDefaultAsync(h => h.Id == id);
        }
    }
}
