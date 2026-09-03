using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Models.Room
{
    public class RoomResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public Guid HotelId { get; set; }
    }
}
