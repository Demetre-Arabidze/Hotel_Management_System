using HMS.Application.Contracts.Persistence;
using HMS.Application.Contracts.Services;
using HMS.Application.Exceptions;
using HMS.Application.Models.Auth;
using HMS.Application.Models.Email;
using HMS.Domain.Entities;
using HMS.Domain.Enums;
using Microsoft.Extensions.Configuration;
using System.Web;

namespace HMS.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IIdentityService _identityService;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IRepositoryBase<Guest> _guestRepository;
        private readonly IRepositoryBase<Manager> _managerRepository;
        private readonly IRepositoryBase<Hotel> _hotelRepository;
        private readonly IEmailService _emailService; // 1. Added Email Service
        private readonly IConfiguration _configuration;

        public AuthService(
            IIdentityService identityService,
            IJwtTokenGenerator jwtTokenGenerator,
            IRepositoryBase<Guest> guestRepository,
            IRepositoryBase<Manager> managerRepository,
            IRepositoryBase<Hotel> hotelRepository,
            IEmailService emailService,
            IConfiguration configuration) // 2. Injected into constructor
        {
            _identityService = identityService;
            _jwtTokenGenerator = jwtTokenGenerator;
            _guestRepository = guestRepository;
            _managerRepository = managerRepository;
            _hotelRepository = hotelRepository;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var (succeeded, userId) = await _identityService.ValidateCredentialsAsync(dto.Email, dto.Password);
            if (!succeeded)
                throw new BadRequestException("Invalid email or password.");

            var roles = await _identityService.GetRolesAsync(userId);
            return await BuildAuthResponse(userId, dto.Email, roles);
        }

        public async Task LogoutAsync(Guid userId)
        {
            await _identityService.RevokeRefreshTokenAsync(userId);
        }

        // 3. New method to handle confirmation requests
        public async Task ConfirmEmailAsync(Guid userId, string token)
        {
            var (succeeded, errors) = await _identityService.ConfirmEmailAsync(userId, token);
            if (!succeeded)
                throw new BadRequestException(string.Join(", ", errors));
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

            await SendConfirmationEmailAsync(userId, dto.Email); // 4. Send email

            var roles = await _identityService.GetRolesAsync(userId);
            return await BuildAuthResponse(userId, dto.Email, roles);
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

            await SendConfirmationEmailAsync(userId, dto.Email); // 4. Send email

            var roles = await _identityService.GetRolesAsync(userId);
            return await BuildAuthResponse(userId, dto.Email, roles);
        }

        public async Task<AuthResponseDto> RegisterAdminAsync(RegisterAdminDto dto)
        {
            var (succeeded, userId, errors) = await _identityService.CreateUserAsync(dto.Email, dto.Password);
            if (!succeeded)
                throw new BadRequestException(string.Join(", ", errors));

            await _identityService.AddToRoleAsync(userId, UserRole.Admin.ToString());

            await SendConfirmationEmailAsync(userId, dto.Email); // 4. Send email

            var roles = await _identityService.GetRolesAsync(userId);
            return await BuildAuthResponse(userId, dto.Email, roles);
        }

        private async Task<AuthResponseDto> BuildAuthResponse(Guid userId, string email, IList<string> roles)
        {
            var (token, expiration) = _jwtTokenGenerator.GenerateToken(userId, email, roles);
            var refreshToken = await _identityService.GenerateRefreshTokenAsync(userId);

            return new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                Expiration = expiration,
                Roles = roles
            };
        }

        // 5. Private helper to handle email generation and sending
        private async Task SendConfirmationEmailAsync(Guid userId, string email)
        {
            var token = await _identityService.GenerateEmailConfirmationTokenAsync(userId);
            var encodedToken = HttpUtility.UrlEncode(token);

            // Read the base URL from configuration (falls back to localhost if missing)
            var baseUrl = _configuration["EmailSettings:BaseUrl"] ?? "http://localhost:5180";

            var confirmationLink = $"{baseUrl}/api/auth/confirm-email?userId={userId}&token={encodedToken}";

            await _emailService.SendEmailAsync(new EmailMessageDto
            {
                To = email,
                Subject = "Confirm your HMS Account",
                Body = $"<h3>Welcome to Hotel Management System!</h3><p>Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.</p>"
            });
        }
    }
}
