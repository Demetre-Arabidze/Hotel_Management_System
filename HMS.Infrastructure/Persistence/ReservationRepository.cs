using HMS.Application.Contracts.Persistence;
using HMS.Domain.Entities;
using HMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Infrastructure.Persistence
{
    public class ReservationRepository
        : RepositoryBase<Reservation>, IReservationRepository
    {
        private readonly ApplicationDbContext _context;

        public ReservationRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<bool> HasRoomConflictAsync(
            IEnumerable<Guid> roomIds,
            DateOnly checkInDate,
            DateOnly checkOutDate,
            Guid? excludedReservationId = null)
        {
            var roomIdList = roomIds.Distinct().ToList();

            if (roomIdList.Count == 0)
                return false;

            return await _context.ReservationRooms.AnyAsync(
                x => roomIdList.Contains(x.RoomId) &&
                     (!excludedReservationId.HasValue ||
                      x.ReservationId != excludedReservationId.Value) &&
                     x.Reservation.CheckInDate < checkOutDate &&
                     x.Reservation.CheckOutDate > checkInDate);
        }

        public async Task<Reservation?> GetWithDetailsAsync(
            Guid reservationId,
            bool tracking = true)
        {
            IQueryable<Reservation> query = _context.Reservations
                .Include(x => x.Guest)
                .Include(x => x.ReservationRooms)
                    .ThenInclude(x => x.Room)
                        .ThenInclude(x => x.Hotel);

            if (!tracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(
                x => x.Id == reservationId);
        }

        public async Task<IReadOnlyList<Reservation>> SearchAsync(
            Guid? hotelId,
            Guid? guestId,
            Guid? roomId,
            DateOnly? reservationDate,
            bool? isActive,
            DateOnly today)
        {
            IQueryable<Reservation> query = _context.Reservations
                .AsNoTracking()
                .Include(x => x.Guest)
                .Include(x => x.ReservationRooms)
                    .ThenInclude(x => x.Room)
                        .ThenInclude(x => x.Hotel);

            if (hotelId.HasValue)
            {
                query = query.Where(x =>
                    x.ReservationRooms.Any(room =>
                        room.Room.HotelId == hotelId.Value));
            }

            if (guestId.HasValue)
                query = query.Where(x => x.GuestId == guestId.Value);

            if (roomId.HasValue)
            {
                query = query.Where(x =>
                    x.ReservationRooms.Any(room =>
                        room.RoomId == roomId.Value));
            }

            if (reservationDate.HasValue)
            {
                query = query.Where(x =>
                    x.CheckInDate <= reservationDate.Value &&
                    x.CheckOutDate > reservationDate.Value);
            }

            if (isActive == true)
                query = query.Where(x => x.CheckOutDate >= today);

            if (isActive == false)
                query = query.Where(x => x.CheckOutDate < today);

            return await query
                .OrderByDescending(x => x.CheckInDate)
                .ToListAsync();
        }

        public async Task<bool> GuestHasActiveOrFutureReservationsAsync(
            Guid guestId,
            DateOnly today)
        {
            return await _context.Reservations.AnyAsync(
                x => x.GuestId == guestId &&
                     x.CheckOutDate >= today);
        }

        public async Task<bool> HotelHasActiveOrFutureReservationsAsync(
            Guid hotelId,
            DateOnly today)
        {
            return await _context.ReservationRooms.AnyAsync(
                x => x.Room.HotelId == hotelId &&
                     x.Reservation.CheckOutDate >= today);
        }
    }
}
