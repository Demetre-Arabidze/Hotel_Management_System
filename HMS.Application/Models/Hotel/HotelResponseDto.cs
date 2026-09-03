using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Models.Hotel
{
    public class HotelResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Rating { get; set; }
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
