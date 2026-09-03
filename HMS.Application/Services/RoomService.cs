using HMS.Application.Contracts.Persistence;
using HMS.Application.Contracts.Services;
using HMS.Application.Exceptions;
using HMS.Application.Models.Room;
using HMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRepositoryBase<Room> _roomRepository;
        private readonly IRepositoryBase<Hotel> _hotelRepository;
        private readonly IReservationRepository _reservationRepository;

        public RoomService(
            IRepositoryBase<Room> roomRepository,
            IRepositoryBase<Hotel> hotelRepository,
            IReservationRepository reservationRepository)
        {
            _roomRepository = roomRepository;
            _hotelRepository = hotelRepository;
            _reservationRepository = reservationRepository;
        }

        public async Task<IEnumerable<RoomResponseDto>> GetHotelRoomsAsync(Guid hotelId, RoomSearchFilterDto filter)
        {
            bool hotelExists = await _hotelRepository.ExistsAsync(h => h.Id == hotelId);
            if (!hotelExists)
                throw new NotFoundException(nameof(Hotel), hotelId);

            var (rooms, _) = await _roomRepository.GetAllAsync(
                filter: r => r.HotelId == hotelId &&
                            (!filter.MinPrice.HasValue || r.Price >= filter.MinPrice.Value) &&
                            (!filter.MaxPrice.HasValue || r.Price <= filter.MaxPrice.Value),
                tracking: false
            );

            var resultList = rooms.ToList();

            if (filter.TargetDate.HasValue)
            {
                var target = filter.TargetDate.Value;
                var availableRooms = new List<Room>();

                foreach (var room in resultList)
                {
                    bool isOccupied = await _reservationRepository.HasRoomConflictAsync(
                        new[] { room.Id },
                        target,
                        target.AddDays(1));

                    if (!isOccupied)
                        availableRooms.Add(room);
                }

                resultList = availableRooms;
            }

            return resultList.Select(r => new RoomResponseDto
            {
                Id = r.Id,
                Name = r.Name,
                Price = r.Price,
                HotelId = r.HotelId
            });
        }

        public async Task<RoomResponseDto> GetByIdAsync(Guid hotelId, Guid roomId)
        {
            var room = await _roomRepository.GetAsync(
                r => r.Id == roomId && r.HotelId == hotelId,
                tracking: false);

            if (room == null)
                throw new NotFoundException(nameof(Room), roomId);

            return new RoomResponseDto
            {
                Id = room.Id,
                Name = room.Name,
                Price = room.Price,
                HotelId = room.HotelId
            };
        }

        public async Task<RoomResponseDto> CreateAsync(Guid hotelId, RoomCreateUpdateDto dto)
        {
            if (dto.Price <= 0)
                throw new BadRequestException("Room price must be greater than zero.");

            bool hotelExists = await _hotelRepository.ExistsAsync(h => h.Id == hotelId);
            if (!hotelExists)
                throw new NotFoundException(nameof(Hotel), hotelId);

            var room = new Room
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Price = dto.Price,
                HotelId = hotelId
            };

            await _roomRepository.AddAsync(room);
            await _roomRepository.SaveAsync();

            return new RoomResponseDto
            {
                Id = room.Id,
                Name = room.Name,
                Price = room.Price,
                HotelId = room.HotelId
            };
        }

        public async Task UpdateAsync(Guid hotelId, Guid roomId, RoomCreateUpdateDto dto)
        {
            if (dto.Price <= 0)
                throw new BadRequestException("Room price must be greater than zero.");

            var room = await _roomRepository.GetAsync(r => r.Id == roomId && r.HotelId == hotelId);
            if (room == null)
                throw new NotFoundException(nameof(Room), roomId);

            room.Name = dto.Name;
            room.Price = dto.Price;

            _roomRepository.Update(room);
            await _roomRepository.SaveAsync();
        }

        public async Task DeleteAsync(Guid hotelId, Guid roomId)
        {
            var room = await _roomRepository.GetAsync(r => r.Id == roomId && r.HotelId == hotelId);
            if (room == null)
                throw new NotFoundException(nameof(Room), roomId);

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            bool hasActiveOrFutureReservations = await _reservationRepository.HasRoomConflictAsync(
                new[] { roomId },
                today,
                DateOnly.MaxValue);

            if (hasActiveOrFutureReservations)
                throw new BadRequestException("Cannot delete room with active or future reservations.");

            _roomRepository.Remove(room);
            await _roomRepository.SaveAsync();
        }
    }
}
