using HMS.Application.Contracts.Services;
using HMS.Application.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace HMS.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public IdentityService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<(bool Succeeded, Guid UserId, IEnumerable<string> Errors)> CreateUserAsync(string email, string password)
        {
            var user = new ApplicationUser { Email = email, UserName = email };
            var result = await _userManager.CreateAsync(user, password);
            return (result.Succeeded, user.Id, result.Errors.Select(e => e.Description));
        }

        public async Task<(bool Succeeded, Guid UserId)> ValidateCredentialsAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                return (false, Guid.Empty);

            if (!await _userManager.IsEmailConfirmedAsync(user))
                throw new BadRequestException("Please confirm your email address before logging in.");

            return (true, user.Id);
        }

        public async Task AddToRoleAsync(Guid userId, string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole<Guid>(role));

            var user = await _userManager.FindByIdAsync(userId.ToString());
            await _userManager.AddToRoleAsync(user!, role);
        }

        public async Task<IList<string>> GetRolesAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return await _userManager.GetRolesAsync(user!);
        }

        public async Task<string> GenerateRefreshTokenAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            // Generate a secure random token
            var refreshToken = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));

            // Store it in Identity's AspNetUserTokens table
            await _userManager.SetAuthenticationTokenAsync(user!, "HMS", "RefreshToken", refreshToken);

            return refreshToken;
        }

        public async Task RevokeRefreshTokenAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                await _userManager.RemoveAuthenticationTokenAsync(user, "HMS", "RefreshToken");
            }
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return await _userManager.GenerateEmailConfirmationTokenAsync(user!);
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> ConfirmEmailAsync(Guid userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return (false, new[] { "User not found." });

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return (result.Succeeded, result.Errors.Select(e => e.Description));
        }
    }
}
