using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Models.Reservation
{
    public class ReservationResponseDto
    {
        public Guid Id { get; set; }
        public Guid GuestId { get; set; }
        public string GuestFullName { get; set; } = string.Empty;
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public List<string> RoomNames { get; set; } = new();
    }
}
