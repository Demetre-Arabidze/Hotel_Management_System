using HMS.Application.Contracts.Persistence;
using HMS.Application.Contracts.Services;
using HMS.Application.Exceptions;
using HMS.Application.Models.Manager;
using HMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Services
{
    public class ManagerService : IManagerService
    {
        private readonly IRepositoryBase<Manager> _managerRepository;
        private readonly IRepositoryBase<Hotel> _hotelRepository;

        public ManagerService(
            IRepositoryBase<Manager> managerRepository,
            IRepositoryBase<Hotel> hotelRepository)
        {
            _managerRepository = managerRepository;
            _hotelRepository = hotelRepository;
        }

        public async Task<IEnumerable<ManagerResponseDto>> GetHotelManagersAsync(Guid hotelId)
        {
            bool hotelExists = await _hotelRepository.ExistsAsync(h => h.Id == hotelId);
            if (!hotelExists)
                throw new NotFoundException(nameof(Hotel), hotelId);

            var (managers, _) = await _managerRepository.GetAllAsync(
                filter: m => m.HotelId == hotelId,
                tracking: false);

            return managers.Select(m => new ManagerResponseDto
            {
                Id = m.Id,
                FirstName = m.FirstName,
                LastName = m.LastName,
                PersonalNumber = m.PersonalNumber,
                Email = m.Email,
                PhoneNumber = m.PhoneNumber,
                HotelId = m.HotelId
            });
        }

        public async Task<ManagerResponseDto> AssignToHotelAsync(Guid hotelId, ManagerCreateUpdateDto dto)
        {
            bool hotelExists = await _hotelRepository.ExistsAsync(h => h.Id == hotelId);
            if (!hotelExists)
                throw new NotFoundException(nameof(Hotel), hotelId);

            bool emailExists = await _managerRepository.ExistsAsync(m => m.Email == dto.Email);
            if (emailExists)
                throw new BadRequestException("Email is already registered.");

            bool personalNumberExists = await _managerRepository.ExistsAsync(m => m.PersonalNumber == dto.PersonalNumber);
            if (personalNumberExists)
                throw new BadRequestException("Personal number is already registered.");

            var manager = new Manager
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PersonalNumber = dto.PersonalNumber,
                PhoneNumber = dto.PhoneNumber,
                HotelId = hotelId
            };

            await _managerRepository.AddAsync(manager);
            await _managerRepository.SaveAsync();

            return new ManagerResponseDto
            {
                Id = manager.Id,
                FirstName = manager.FirstName,
                LastName = manager.LastName,
                PersonalNumber = manager.PersonalNumber,
                Email = manager.Email,
                PhoneNumber = manager.PhoneNumber,
                HotelId = manager.HotelId
            };
        }

        public async Task UpdateAsync(Guid hotelId, Guid managerId, ManagerCreateUpdateDto dto)
        {
            var manager = await _managerRepository.GetAsync(m => m.Id == managerId && m.HotelId == hotelId);
            if (manager == null)
                throw new NotFoundException(nameof(Manager), managerId);

            bool emailExists = await _managerRepository.ExistsAsync(m => m.Email == dto.Email && m.Id != managerId);
            if (emailExists)
                throw new BadRequestException("Email is already in use by another manager.");

            bool personalNumberExists = await _managerRepository.ExistsAsync(m => m.PersonalNumber == dto.PersonalNumber && m.Id != managerId);
            if (personalNumberExists)
                throw new BadRequestException("Personal number is already in use by another manager.");

            manager.FirstName = dto.FirstName;
            manager.LastName = dto.LastName;
            manager.Email = dto.Email;
            manager.PersonalNumber = dto.PersonalNumber;
            manager.PhoneNumber = dto.PhoneNumber;

            _managerRepository.Update(manager);
            await _managerRepository.SaveAsync();
        }

        public async Task DeleteAsync(Guid hotelId, Guid managerId)
        {
            var manager = await _managerRepository.GetAsync(m => m.Id == managerId && m.HotelId == hotelId);
            if (manager == null)
                throw new NotFoundException(nameof(Manager), managerId);

            var (_, totalCount) = await _managerRepository.GetAllAsync(m => m.HotelId == hotelId);
            if (totalCount <= 1)
                throw new BadRequestException("Cannot delete manager. A hotel must have at least one manager.");

            _managerRepository.Remove(manager);
            await _managerRepository.SaveAsync();
        }
    }
}
