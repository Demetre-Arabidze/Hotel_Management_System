namespace HMS.Application.Contracts.Services
{
    public interface IJwtTokenGenerator
    {
        (string Token, DateTime Expiration) GenerateToken(Guid userId, string email, IEnumerable<string> roles);
    }
}