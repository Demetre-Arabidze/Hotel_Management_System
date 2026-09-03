using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Models.Manager
{
    public class ManagerResponseDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PersonalNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public Guid HotelId { get; set; }
    }
}
