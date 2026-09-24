using HMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Contracts.Persistence
{
    public interface IRoomRepository : IRepositoryBase<Room>
    {
        Task<IReadOnlyList<Room>> GetAvailableRoomsAsync(
            Guid hotelId,
            decimal? minPrice,
            decimal? maxPrice,
            DateOnly checkInDate,
            DateOnly checkOutDate);

        Task<bool> IsAvailableAsync(
            Guid roomId,
            DateOnly checkInDate,
            DateOnly checkOutDate);

        Task<bool> HasActiveOrFutureReservationsAsync(
            Guid roomId,
            DateOnly today);

        Task<bool> DoRoomsBelongToHotelAsync(IEnumerable<Guid> roomIds, Guid hotelId);
    }
}
