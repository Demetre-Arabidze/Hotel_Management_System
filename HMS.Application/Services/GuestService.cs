using HMS.Application.Contracts.Persistence;
using HMS.Application.Contracts.Services;
using HMS.Application.Exceptions;
using HMS.Application.Models.Guest;
using HMS.Domain.Entities;

namespace HMS.Application.Services
{
    public class GuestService : IGuestService
    {
        private readonly IRepositoryBase<Guest> _guestRepository;
        private readonly IReservationRepository _reservationRepository;

        public GuestService(
            IRepositoryBase<Guest> guestRepository,
            IReservationRepository reservationRepository)
        {
            _guestRepository = guestRepository;
            _reservationRepository = reservationRepository;
        }

        public async Task<GuestResponseDto> GetByIdAsync(Guid id)
        {
            var guest = await _guestRepository.GetAsync(g => g.Id == id, tracking: false);
            if (guest == null)
                throw new NotFoundException(nameof(Guest), id);

            return new GuestResponseDto
            {
                Id = guest.Id,
                FirstName = guest.FirstName,
                LastName = guest.LastName,
                PersonalNumber = guest.PersonalNumber,
                PhoneNumber = guest.PhoneNumber
            };
        }

        public async Task<GuestResponseDto> CreateAsync(GuestCreateUpdateDto dto)
        {
            bool personalNumberExists = await _guestRepository.ExistsAsync(g => g.PersonalNumber == dto.PersonalNumber);
            if (personalNumberExists)
                throw new BadRequestException("Guest with this personal number already exists.");

            bool phoneExists = await _guestRepository.ExistsAsync(g => g.PhoneNumber == dto.PhoneNumber);
            if (phoneExists)
                throw new BadRequestException("Guest with this phone number already exists.");

            var guest = new Guest
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PersonalNumber = dto.PersonalNumber,
                PhoneNumber = dto.PhoneNumber
            };

            await _guestRepository.AddAsync(guest);
            await _guestRepository.SaveAsync();

            return new GuestResponseDto
            {
                Id = guest.Id,
                FirstName = guest.FirstName,
                LastName = guest.LastName,
                PersonalNumber = guest.PersonalNumber,
                PhoneNumber = guest.PhoneNumber
            };
        }

        public async Task UpdateAsync(Guid id, GuestCreateUpdateDto dto)
        {
            var guest = await _guestRepository.GetAsync(g => g.Id == id);
            if (guest == null)
                throw new NotFoundException(nameof(Guest), id);

            bool personalNumberExists = await _guestRepository.ExistsAsync(g => g.PersonalNumber == dto.PersonalNumber && g.Id != id);
            if (personalNumberExists)
                throw new BadRequestException("Personal number is already in use by another guest.");

            bool phoneExists = await _guestRepository.ExistsAsync(g => g.PhoneNumber == dto.PhoneNumber && g.Id != id);
            if (phoneExists)
                throw new BadRequestException("Phone number is already in use by another guest.");

            guest.FirstName = dto.FirstName;
            guest.LastName = dto.LastName;
            guest.PersonalNumber = dto.PersonalNumber;
            guest.PhoneNumber = dto.PhoneNumber;

            _guestRepository.Update(guest);
            await _guestRepository.SaveAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var guest = await _guestRepository.GetAsync(g => g.Id == id);
            if (guest == null)
                throw new NotFoundException(nameof(Guest), id);

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            bool hasReservations = await _reservationRepository.GuestHasActiveOrFutureReservationsAsync(id, today);
            if (hasReservations)
                throw new BadRequestException("Cannot delete guest with active or future reservations.");

            _guestRepository.Remove(guest);
            await _guestRepository.SaveAsync();
        }
    }
}
