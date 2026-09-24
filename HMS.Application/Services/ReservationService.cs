using HMS.Application.Contracts.Persistence;
using HMS.Application.Contracts.Services;
using HMS.Application.Exceptions;
using HMS.Application.Models.Reservation;
using HMS.Domain.Entities;

namespace HMS.Application.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IRepositoryBase<Guest> _guestRepository;

        public ReservationService(
            IReservationRepository reservationRepository,
            IRoomRepository roomRepository,
            IRepositoryBase<Guest> guestRepository)
        {
            _reservationRepository = reservationRepository;
            _roomRepository = roomRepository;
            _guestRepository = guestRepository;
        }

        public async Task<Guid> CreateAsync(Guid hotelId, Guid userId, ReservationCreateDto dto)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (dto.CheckInDate < today)
                throw new BadRequestException("Check-in date cannot be in the past.");

            if (dto.CheckOutDate <= dto.CheckInDate)
                throw new BadRequestException("Check-out date must be after the check-in date.");

            if (dto.RoomIds == null || !dto.RoomIds.Any())
                throw new BadRequestException("At least one room must be selected.");

            // 1. HOTEL-ROOM VALIDATION: Ensure all room IDs exist under the route's hotelId
            bool doRoomsBelongToHotel = await _roomRepository.DoRoomsBelongToHotelAsync(dto.RoomIds, hotelId);
            if (!doRoomsBelongToHotel)
                throw new BadRequestException("One or more selected rooms do not belong to the specified hotel.");

            // 2. SECURE GUEST RESOLUTION
            var guest = await _guestRepository.GetAsync(g => g.UserId == userId);
            if (guest == null)
                throw new Exception("Guest profile not found for the current user.");

            // 3. OVERLAP CHECK
            bool hasConflict = await _reservationRepository.HasRoomConflictAsync(
                dto.RoomIds,
                dto.CheckInDate,
                dto.CheckOutDate);

            if (hasConflict)
                throw new BadRequestException("One or more selected rooms are unavailable for these dates.");

            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                GuestId = guest.Id,
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                ReservationRooms = dto.RoomIds.Select(roomId => new ReservationRoom
                {
                    RoomId = roomId
                }).ToList()
            };

            await _reservationRepository.AddAsync(reservation);
            await _reservationRepository.SaveAsync();

            return reservation.Id;
        }

        public async Task UpdateDatesAsync(Guid reservationId, ReservationUpdateDatesDto dto)
        {
            var reservation = await _reservationRepository.GetWithDetailsAsync(reservationId);
            if (reservation == null)
                throw new NotFoundException(nameof(Reservation), reservationId);

            if (dto.CheckOutDate <= dto.CheckInDate)
                throw new BadRequestException("Check-out date must be after the check-in date.");

            var roomIds = reservation.ReservationRooms.Select(rr => rr.RoomId);

            bool hasConflict = await _reservationRepository.HasRoomConflictAsync(
                roomIds,
                dto.CheckInDate,
                dto.CheckOutDate,
                excludedReservationId: reservationId);

            if (hasConflict)
                throw new BadRequestException("The updated dates overlap with another existing booking.");

            reservation.CheckInDate = dto.CheckInDate;
            reservation.CheckOutDate = dto.CheckOutDate;

            _reservationRepository.Update(reservation);
            await _reservationRepository.SaveAsync();
        }

        public async Task CancelAsync(Guid reservationId)
        {
            var reservation = await _reservationRepository.GetAsync(r => r.Id == reservationId);
            if (reservation == null)
                throw new NotFoundException(nameof(Reservation), reservationId);

            _reservationRepository.Remove(reservation);
            await _reservationRepository.SaveAsync();
        }
    }
}
