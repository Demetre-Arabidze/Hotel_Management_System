using HMS.Application.Models.Reservation;

namespace HMS.Application.Contracts.Services
{
    public interface IReservationService
    {
        Task<Guid> CreateAsync(Guid hotelId, Guid userId, ReservationCreateDto dto);
        Task UpdateDatesAsync(Guid reservationId, ReservationUpdateDatesDto dto);
        Task CancelAsync(Guid reservationId);
    }
}
