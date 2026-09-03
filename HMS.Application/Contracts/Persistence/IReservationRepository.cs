using HMS.Domain.Entities;

namespace HMS.Application.Contracts.Persistence
{
    public interface IReservationRepository : IRepositoryBase<Reservation>
    {
        Task<bool> HasRoomConflictAsync(
            IEnumerable<Guid> roomIds,
            DateOnly checkInDate,
            DateOnly checkOutDate,
            Guid? excludedReservationId = null);

        Task<Reservation?> GetWithDetailsAsync(
            Guid reservationId,
            bool tracking = true);

        Task<IReadOnlyList<Reservation>> SearchAsync(
            Guid? hotelId,
            Guid? guestId,
            Guid? roomId,
            DateOnly? reservationDate,
            bool? isActive,
            DateOnly today);

        Task<bool> GuestHasActiveOrFutureReservationsAsync(
            Guid guestId,
            DateOnly today);

        Task<bool> HotelHasActiveOrFutureReservationsAsync(
            Guid hotelId,
            DateOnly today      );
    }
}
