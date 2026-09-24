using HMS.Application.Contracts.Persistence;
using HMS.Application.Contracts.Services;
using HMS.Application.Exceptions;
using HMS.Application.Models.Auth;
using HMS.Domain.Entities;
using HMS.Domain.Enums;

namespace HMS.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IIdentityService _identityService;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IRepositoryBase<Guest> _guestRepository;
        private readonly IRepositoryBase<Manager> _managerRepository;
        private readonly IRepositoryBase<Hotel> _hotelRepository;

        public AuthService(
            IIdentityService identityService,
            IJwtTokenGenerator jwtTokenGenerator,
            IRepositoryBase<Guest> guestRepository,
            IRepositoryBase<Manager> managerRepository,
            IRepositoryBase<Hotel> hotelRepository)
        {
            _identityService = identityService;
            _jwtTokenGenerator = jwtTokenGenerator;
            _guestRepository = guestRepository;
            _managerRepository = managerRepository;
            _hotelRepository = hotelRepository;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var (succeeded, userId) = await _identityService.ValidateCredentialsAsync(dto.Email, dto.Password);
            if (!succeeded)
                throw new BadRequestException("Invalid email or password.");

            var roles = await _identityService.GetRolesAsync(userId);
            return BuildAuthResponse(userId, dto.Email, roles);
        }

        public async Task<AuthResponseDto> RegisterGuestAsync(RegisterGuestDto dto)
        {
            if (await _guestRepository.ExistsAsync(g => g.PersonalNumber == dto.PersonalNumber))
                throw new BadRequestException("Personal number is already in use.");

            var (succeeded, userId, errors) = await _identityService.CreateUserAsync(dto.Email, dto.Password);
            if (!succeeded)
                throw new BadRequestException(string.Join(", ", errors));

            await _identityService.AddToRoleAsync(userId, UserRole.Guest.ToString());

            var guest = new Guest
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PersonalNumber = dto.PersonalNumber,
                PhoneNumber = dto.PhoneNumber
            };

            await _guestRepository.AddAsync(guest);
            await _guestRepository.SaveAsync();

            var roles = await _identityService.GetRolesAsync(userId);
            return BuildAuthResponse(userId, dto.Email, roles);
        }

        public async Task<AuthResponseDto> RegisterManagerAsync(RegisterManagerDto dto)
        {
            if (!await _hotelRepository.ExistsAsync(h => h.Id == dto.HotelId))
                throw new NotFoundException(nameof(Hotel), dto.HotelId);

            if (await _managerRepository.ExistsAsync(m => m.PersonalNumber == dto.PersonalNumber))
                throw new BadRequestException("Personal number is already in use.");

            var (succeeded, userId, errors) = await _identityService.CreateUserAsync(dto.Email, dto.Password);
            if (!succeeded)
                throw new BadRequestException(string.Join(", ", errors));

            await _identityService.AddToRoleAsync(userId, UserRole.Manager.ToString());

            var manager = new Manager
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PersonalNumber = dto.PersonalNumber,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                HotelId = dto.HotelId
            };

            await _managerRepository.AddAsync(manager);
            await _managerRepository.SaveAsync();

            var roles = await _identityService.GetRolesAsync(userId);
            return BuildAuthResponse(userId, dto.Email, roles);
        }

        public async Task<AuthResponseDto> RegisterAdminAsync(RegisterAdminDto dto)
        {
            var (succeeded, userId, errors) = await _identityService.CreateUserAsync(dto.Email, dto.Password);
            if (!succeeded)
                throw new BadRequestException(string.Join(", ", errors));

            await _identityService.AddToRoleAsync(userId, UserRole.Admin.ToString());

            var roles = await _identityService.GetRolesAsync(userId);
            return BuildAuthResponse(userId, dto.Email, roles);
        }

        private AuthResponseDto BuildAuthResponse(Guid userId, string email, IList<string> roles)
        {
            var (token, expiration) = _jwtTokenGenerator.GenerateToken(userId, email, roles);

            return new AuthResponseDto
            {
                Token = token,
                Expiration = expiration,
                Roles = roles
            };
        }
    }
}
