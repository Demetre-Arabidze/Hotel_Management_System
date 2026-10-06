using HMS.Application.Models.Email;

namespace HMS.Application.Contracts.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessageDto email);
    }
}
