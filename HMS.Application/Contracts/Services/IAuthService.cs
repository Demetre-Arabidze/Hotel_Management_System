using HMS.Application.Models.Auth;

namespace HMS.Application.Contracts.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> RegisterGuestAsync(RegisterGuestDto dto);
        Task<AuthResponseDto> RegisterManagerAsync(RegisterManagerDto dto);
        Task<AuthResponseDto> RegisterAdminAsync(RegisterAdminDto dto);
    }
}
