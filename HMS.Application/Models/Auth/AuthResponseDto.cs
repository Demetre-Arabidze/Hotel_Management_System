namespace HMS.Application.Models.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
