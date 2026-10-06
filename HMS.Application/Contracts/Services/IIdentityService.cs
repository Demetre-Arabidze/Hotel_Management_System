namespace HMS.Application.Contracts.Services
{
    public interface IIdentityService
    {
        Task<(bool Succeeded, Guid UserId, IEnumerable<string> Errors)> CreateUserAsync(string email, string password);
        Task<(bool Succeeded, Guid UserId)> ValidateCredentialsAsync(string email, string password);
        Task AddToRoleAsync(Guid userId, string role);
        Task<IList<string>> GetRolesAsync(Guid userId);
        Task<string> GenerateRefreshTokenAsync(Guid userId);
        Task RevokeRefreshTokenAsync(Guid userId);
        Task<string> GenerateEmailConfirmationTokenAsync(Guid userId);
        Task<(bool Succeeded, IEnumerable<string> Errors)> ConfirmEmailAsync(Guid userId, string token);
    }
}
