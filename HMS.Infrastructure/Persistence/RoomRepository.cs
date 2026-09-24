using HMS.Application.Contracts.Persistence;
using HMS.Domain.Entities;
using HMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Infrastructure.Persistence
{
    public class RoomRepository : RepositoryBase<Room>, IRoomRepository
    {
        private readonly ApplicationDbContext _context;

        public RoomRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Room>> GetAvailableRoomsAsync(
            Guid hotelId,
            decimal? minPrice,
            decimal? maxPrice,
            DateOnly checkInDate,
            DateOnly checkOutDate)
        {
            var bookedRoomIds = _context.ReservationRooms
                .Where(x =>
                    x.Reservation.CheckInDate < checkOutDate &&
                    x.Reservation.CheckOutDate > checkInDate)
                .Select(x => x.RoomId);

            IQueryable<Room> query = _context.Rooms
                .AsNoTracking()
                .Where(x => x.HotelId == hotelId);

            if (minPrice.HasValue)
                query = query.Where(x => x.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(x => x.Price <= maxPrice.Value);

            query = query
                .Where(x => !bookedRoomIds.Contains(x.Id))
                .OrderBy(x => x.Price);

            return await query.ToListAsync();
        }

        public async Task<bool> IsAvailableAsync(
            Guid roomId,
            DateOnly checkInDate,
            DateOnly checkOutDate)
        {
            return !await _context.ReservationRooms.AnyAsync(
                x => x.RoomId == roomId &&
                     x.Reservation.CheckInDate < checkOutDate &&
                     x.Reservation.CheckOutDate > checkInDate);
        }

        public async Task<bool> HasActiveOrFutureReservationsAsync(
            Guid roomId,
            DateOnly today)
        {
            return await _context.ReservationRooms.AnyAsync(
                x => x.RoomId == roomId &&
                     x.Reservation.CheckOutDate >= today);
        }

        public async Task<bool> DoRoomsBelongToHotelAsync(IEnumerable<Guid> roomIds, Guid hotelId)
        {
            var distinctRoomIds = roomIds.Distinct().ToList();

            // Query SQL Server with a single COUNT query
            var validCount = await _context.Rooms
                .Where(r => distinctRoomIds.Contains(r.Id) && r.HotelId == hotelId)
                .CountAsync();

            return validCount == distinctRoomIds.Count;
        }
    }
}
